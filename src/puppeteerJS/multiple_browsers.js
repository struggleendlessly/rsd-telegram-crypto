import puppeteer from 'puppeteer';
import fs from 'fs/promises';
import { setTimeout as wait } from 'timers/promises';

const LOCAL_STORAGE_FILE = './localStorageData.json';
const COOKIES_FILE = './cookies.json';
const TELEGRAM_URL = 'https://web.telegram.org/k/';
const TELEGRAM_CHAT_URLS = [
  'https://web.telegram.org/k/#-2294837322',
  'https://web.telegram.org/k/#-5078022809',
  'https://web.telegram.org/k/#-5078721003'
];

async function saveSession(page) {
  const localStorageData = await page.evaluate(() => {
    const json = {};
    for (let i = 0; i < localStorage.length; i++) {
      const key = localStorage.key(i);
      json[key] = localStorage.getItem(key);
    }
    return json;
  });
  await fs.writeFile(LOCAL_STORAGE_FILE, JSON.stringify(localStorageData, null, 2));

  const cookies = await page.cookies();
  await fs.writeFile(COOKIES_FILE, JSON.stringify(cookies, null, 2));

  console.log('Session data saved!');
}

async function loadSession(page) {
  try {
    const localStorageData = JSON.parse(await fs.readFile(LOCAL_STORAGE_FILE));
    const cookies = JSON.parse(await fs.readFile(COOKIES_FILE));

    await page.setCookie(...cookies);

    await page.goto(TELEGRAM_URL, { waitUntil: 'domcontentloaded' });

    await page.evaluate((data) => {
      for (const key in data) {
        localStorage.setItem(key, data[key]);
      }
    }, localStorageData);

    console.log('Session data restored!');
    return true;
  } catch (err) {
    console.log('No existing session found, please authenticate manually.');
    await page.goto(TELEGRAM_URL);
    return false;
  }
}

async function openTelegramBrowser(chatUrl) {
  const browser = await puppeteer.launch({
    headless: false,
    args: [
      '--disable-background-timer-throttling',
      '--disable-renderer-backgrounding',
      '--disable-backgrounding-occluded-windows',
      '--start-maximized',
      '--disable-web-security',
      '--allow-running-insecure-content',
      '--ignore-certificate-errors'
    ],
  });

  const page = await browser.newPage();
  let previousMessage = '';

  // Capture page console messages
  page.on('console', async (msg) => {
    const tag = '[CHAT MESSAGE]';
    const messageText = msg.text();
    if (messageText.includes(tag) && messageText !== previousMessage) {
      // log in green in VS Code
      console.log(`\x1b[32m${messageText}\x1b[0m`);
      previousMessage = messageText;
    }
  });

  const sessionLoaded = await loadSession(page);

  if (!sessionLoaded) {
    console.log('Waiting 60s for manual authentication...');
    await wait(60000);
    await saveSession(page);
  }

  await wait(5000); // give time for session to settle
  await page.goto(chatUrl, { waitUntil: 'domcontentloaded' });

  // Inject custom logging script
  await page.addScriptTag({ path: './script.js' });
  await page.addScriptTag({
    content: `
      console.log('[Custom Logger] Script injected successfully!');
      const originalLog = console.log;
      console.log = (...args) => {
        originalLog('[Telegram]', ...args);
      };
    `
  });

  console.log(`Browser opened and navigated to chat: ${chatUrl}`);
  return browser;
}

(async () => {
  const browsers = [];

  for (const url of TELEGRAM_CHAT_URLS) {
    const browser = await openTelegramBrowser(url);
    browsers.push(browser);
  }

  console.log('All 3 browsers are running sequentially. Listening for messages...');
})();
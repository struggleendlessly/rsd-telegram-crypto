//SAVE TELEGRAM SESSION
import puppeteer from 'puppeteer';
import fs from 'fs/promises';
import { setTimeout as wait } from 'timers/promises';

const LOCAL_STORAGE_FILE = './localStorageData.json';
const COOKIES_FILE = './cookies.json';
const TELEGRAM_URL = 'https://web.telegram.org/k/';
const TELEGRAM_CHAT_URL = 'https://web.telegram.org/k/#-5078721003';

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

async function sessionFilesExist() {
  try {
    await fs.access(LOCAL_STORAGE_FILE);
    await fs.access(COOKIES_FILE);
    return true;
  } catch {
    return false;
  }
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

(async () => {
    const hasSession = await sessionFilesExist();
    const browser = await puppeteer.launch({
    headless: hasSession,
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

  page.on('console', async (msg) => {
    const tag = '[CHAT MESSAGE]'
    const messageText = msg.text()
    if (messageText.includes(tag) && messageText !== previousMessage) {
      console.log(messageText);
      previousMessage = messageText;
    }
  });

  const loadSessionResult = await loadSession(page);

  if (loadSessionResult) {
    await wait(10000);
    await page.goto(TELEGRAM_CHAT_URL, { waitUntil: 'domcontentloaded' });
    await page.addScriptTag({ path: './scraper_script.js' });

    await page.addScriptTag({
      content: `
      console.log('[Custom Logger] Script injected successfully!');
      const originalLog = console.log;
      console.log = (...args) => {
        originalLog('[Telegram]', ...args);
      };
    `
    });

    console.log('Chat tab opened, script was added.');
    return;
  }

  console.log('Waiting 60s for manual authentication (if needed)...');
  await wait(60000);

  await saveSession(page);
  console.log('Ready! You can restart now to reuse session.');

})();
// RUN SINGLE BROWSER WINDOW AND ATTACH SCRIPT
// const puppeteer = require('puppeteer');

// (async () => {
//   const chromeArgs = [
//     '--disable-background-timer-throttling',
//     '--disable-renderer-backgrounding',
//     '--disable-backgrounding-occluded-windows',
//     '--start-maximized',
//     '--disable-web-security',
//     '--allow-running-insecure-content',
//     '--ignore-certificate-errors'
//   ];

//   const browser = await puppeteer.launch({
//     headless: false, // must be visible for Telegram rendering
//     args: chromeArgs
//   });
//   const page = await browser.newPage();
//   await page.goto('https://web.telegram.org/k/', { waitUntil: 'networkidle2' });

//   // // Inject your script here
//   // await page.addScriptTag({ path: './script.js' });
// })();


// LOAD SEVERAL BROWSER WINDOWS
// const puppeteer = require('puppeteer');

// (async () => {
//   const chromeArgs = [
//     '--disable-background-timer-throttling',
//     '--disable-renderer-backgrounding',
//     '--disable-backgrounding-occluded-windows',
//     '--start-maximized',
//     '--disable-web-security',
//     '--allow-running-insecure-content',
//     '--ignore-certificate-errors'
//   ];

//   const browsers = await Promise.all([
//     puppeteer.launch({ headless: false, args: chromeArgs }),
//     puppeteer.launch({ headless: false, args: chromeArgs }),
//     puppeteer.launch({ headless: false, args: chromeArgs }),
//   ]);

//   const pages = await Promise.all(
//     browsers.map(async (browser, i) => {
//       const page = await browser.newPage();
//       await page.goto('https://web.telegram.org/k/', { waitUntil: 'networkidle2' });
//       console.log(`Browser ${i + 1} ready`);
//       return page;
//     })
//   );
// })();

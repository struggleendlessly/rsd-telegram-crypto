document.getElementById("start").addEventListener("click", async () => {
    let [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
    if (tab) {
        chrome.storage.local.get(["runningTabs"], (data) => {
            let runningTabs = data.runningTabs || {};
            runningTabs[tab.id] = true;
            chrome.storage.local.set({ runningTabs });
        });

        console.log(`Автообновление включено для вкладки ${tab.id}`);

        chrome.scripting.executeScript({
            target: { tabId: tab.id },
            files: ["code.js"]
        });
    }
});

document.getElementById("stop").addEventListener("click", async () => {
    let [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
    if (tab) {
        chrome.storage.local.get(["runningTabs"], (data) => {
            let runningTabs = data.runningTabs || {};
            delete runningTabs[tab.id];
            chrome.storage.local.set({ runningTabs });
        });

        console.log(`Автообновление отключено для вкладки ${tab.id}`);
    }
});
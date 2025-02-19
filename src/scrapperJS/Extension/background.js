let activeTabs = {};

chrome.storage.local.get(["runningTabs"], (data) => {
    if (data.runningTabs) {
        Object.keys(data.runningTabs).forEach((tabId) => {
            startAutoReload(parseInt(tabId, 10));
        });
    }
});

chrome.storage.onChanged.addListener((changes) => {
    if (changes.runningTabs) {
        let runningTabs = changes.runningTabs.newValue || {};
        Object.keys(activeTabs).forEach((tabId) => {
            if (!runningTabs[tabId]) stopAutoReload(parseInt(tabId, 10));
        });
        Object.keys(runningTabs).forEach((tabId) => {
            if (!activeTabs[tabId]) startAutoReload(parseInt(tabId, 10));
        });
    }
});

function startAutoReload(tabId) {
    stopAutoReload(tabId);

    console.log(`[${new Date().toLocaleTimeString()}] Запуск автообновления для вкладки ${tabId}`);

    activeTabs[tabId] = setInterval(() => {
        console.log(`[${new Date().toLocaleTimeString()}] Перезагрузка вкладки ${tabId}`);
        chrome.tabs.reload(tabId);
    }, 20000);
}

chrome.tabs.onUpdated.addListener((tabId, changeInfo) => {
    if (activeTabs[tabId] && changeInfo.status === "complete") {
        console.log(`[${new Date().toLocaleTimeString()}] Выполняем code.js на вкладке ${tabId}`);
        chrome.scripting.executeScript({
            target: { tabId },
            files: ["code.js"]
        });
    }
});

chrome.tabs.onRemoved.addListener((tabId) => {
    stopAutoReload(tabId);
});

function stopAutoReload(tabId) {
    if (activeTabs[tabId]) {
        clearInterval(activeTabs[tabId]);
        delete activeTabs[tabId];

        console.log(`[${new Date().toLocaleTimeString()}] Остановлена перезагрузка для вкладки ${tabId}`);

        chrome.storage.local.get(["runningTabs"], (data) => {
            let runningTabs = data.runningTabs || {};
            delete runningTabs[tabId];
            chrome.storage.local.set({ runningTabs });
        });
    }
}

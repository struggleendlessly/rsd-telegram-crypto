let activeTabs = {};

chrome.storage.local.get(["runningTabs"], (data) => {
    if (data.runningTabs) {
        Object.keys(data.runningTabs).forEach((tabId) => {
            startAutoReload(parseInt(tabId, 10), data.runningTabs[tabId]);
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
            if (!activeTabs[tabId]) startAutoReload(parseInt(tabId, 10), runningTabs[tabId]);
        });
    }
});

function getRandomizedInterval(baseMinutes) {
    let min = baseMinutes - 10;
    let max = baseMinutes + 10;
    return (Math.floor(Math.random() * (max - min + 1)) + min) * 60000; // в миллисекундах
}

function startAutoReload(tabId, baseInterval) {
    stopAutoReload(tabId);

    let interval = getRandomizedInterval(baseInterval);
    console.log(`[${new Date().toLocaleTimeString()}] Запуск автообновления вкладки ${tabId} с интервалом ${interval / 60000} мин`);
    debugger;

    activeTabs[tabId] = setInterval(() => {
        let newInterval = getRandomizedInterval(baseInterval);
        console.log(`[${new Date().toLocaleTimeString()}] Перезагрузка вкладки ${tabId}, следующий интервал: ${newInterval / 60000} мин`);
        chrome.tabs.reload(tabId);
        clearInterval(activeTabs[tabId]); // Очищаем старый интервал
        activeTabs[tabId] = setInterval(() => startAutoReload(tabId, baseInterval), newInterval);
    }, interval);
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

document.getElementById("save").addEventListener("click", () => {
    let interval = parseInt(document.getElementById("interval").value, 10);
    if (isNaN(interval) || interval < 1) {
        alert("Введите корректное значение!");
        return;
    }
    chrome.storage.local.set({ reloadInterval: interval });
    console.log(`Интервал обновления сохранен: ${interval} мин`);
});

document.getElementById("start").addEventListener("click", async () => {
    let [tab] = await chrome.tabs.query({ active: true, currentWindow: true });
    if (tab) {
        chrome.storage.local.get(["runningTabs", "reloadInterval"], (data) => {
            let runningTabs = data.runningTabs || {};
            runningTabs[tab.id] = data.reloadInterval || 60; // Используем сохраненный интервал, иначе 60 мин
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

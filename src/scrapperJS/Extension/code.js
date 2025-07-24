// Function to log "trigger" to the console when a new message appears
function logNewMessagePhanes(doc, chatTitle) {
    var elements = Array.from(doc.querySelectorAll('.translatable-message'));

    var element = elements.reverse().find(element => !/^[A-Za-z0-9]{32,}$/.test(element.textContent.trim()));

    if (!element) {
        console.error('Element not found');
        return;
    }

    var valueNameElement = element.querySelector('a.anchor-url[href*="price_"] strong');
    var valueName = valueNameElement ? valueNameElement.textContent.trim() : null;
    console.log('Extracted name:', valueName);

    var valueAddressElement = element.querySelector('code.monospace-text');
    var valueAddress = valueAddressElement ? valueAddressElement.textContent.trim() : null;
    console.log('Extracted address:', valueAddress);

    var arrList = element.querySelectorAll('code.monospace-text');

    var numericValueElement = Array.from(arrList)
        .find(el => el.innerText.includes('MC'));

    var formattedMarketCap = numericValueElement ? numericValueElement.nextSibling.textContent
        .trim().replace(/\$/g, '').split(':').pop().trim() : null;

    console.log('Extracted value:', formattedMarketCap);

    var numericValue = convertToNumeric(formattedMarketCap);

    const networkElement = Array.from(element.querySelectorAll('a.anchor-hashtag'))
        .find(el => el.textContent.includes('#'));
    const network = networkElement ? networkElement.textContent.trim().replace('#', '') : null;

    console.log("Network:", network);

    console.log('Phanes PostRequest:',chatTitle, valueName, numericValue, valueAddress, network);

    if (valueName !== null && numericValue !== null && valueAddress !== null && network !== null) {
        sendPOSTRequest(valueName, numericValue, valueAddress, network, chatTitle);
    }
}

function convertToNumeric(valueMk) {
    if (!valueMk) return null;

    var multiplier = 1;

    if (valueMk.includes('K')) {
        multiplier = 1000;
    } else if (valueMk.includes('M')) {
        multiplier = 1000000;
    }

    var numericValue = parseFloat(
        valueMk.replace('$', '').replace(/,/g, '').replace(/[KM]/, '')
    ) * multiplier;

    console.log('Converted value:', numericValue);
    return numericValue;
}

function logNewMessageRick(doc, chatTitle) {
    var elements = Array.from(doc.querySelectorAll('.translatable-message'));
    
    var element = elements.reverse().find(element => !/^[A-Za-z0-9]{32,}$/.test(element.textContent.trim()));

    if (!element) {
        console.error('Element not found');
        return;
    }

    var valueNameElement = element.querySelector('a.anchor-url strong');
    var valueName = valueNameElement ? valueNameElement.textContent.trim() : null;
    console.log('Extracted name:', valueName);

    var valueAddressElement = element.querySelectorAll('code.monospace-text');

    var last = valueAddressElement[valueAddressElement.length- 1];

    var valueAddress = last ? last.textContent.trim() : null;
    console.log('Extracted address:', valueAddress);

    var emojis =  doc.querySelectorAll('.emoji');

    var targetImgValue = Array.from(emojis)
    .find(el => (el.innerText && el.innerText.includes('💎')) || (el.alt && el.alt.includes('💎')));

    var formattedMarketCap = "";

    if (targetImgValue) {
        var formattedMarketCap = targetImgValue.nextElementSibling.textContent.trim();

        console.log("Extracted address:", formattedMarketCap);
    } else {
        console.log("Target <img> not found");
    }

    var numericValue = convertToNumeric(formattedMarketCap);

    var targetImgNetwork = Array.from(emojis)
    .find(el => (el.innerText && el.innerText.includes('🌐')) || (el.alt && el.alt.includes('🌐')));

    var network = "";

    if (targetImgNetwork) {
        var network = targetImgNetwork.nextSibling.textContent.trim();
    
        console.log("Network:", network);
    } else {
        console.log("Target <img> not found");
    }

    console.log('Rick PostRequest:',chatTitle, valueName, numericValue, valueAddress, network);
    sendPOSTRequest(valueName, numericValue, valueAddress, network, chatTitle);
}

function logNewMessageMonke(doc, chatTitle) {
    var element = doc.querySelector('.translatable-message');

    if (!element) {
        console.error('Element not found');
        return;
    }

    var valueNameElement = element.querySelector('a.anchor-url[href*="dexscreener.com"]');
    var valueName = valueNameElement ? valueNameElement.textContent.split(' - ')[0].trim() : null;

    console.log('Extracted name:', valueName);

    var valueAddressElement = element.querySelector('code.monospace-text');
    var valueAddress = valueAddressElement ? valueAddressElement.textContent.trim() : null;

    console.log('Extracted address:', valueAddress);

    var numericValueElement = Array.from(element.querySelectorAll('strong'))
        .find(el => el.textContent.includes('Market Cap (FDV)'));
    var formattedMarketCap = numericValueElement ? numericValueElement.nextSibling.textContent
    .trim().replace(/\$/g, '').split(':').pop().trim() : null;
    
    console.log('Extracted value:', formattedMarketCap);
    
    const networkElement = Array.from(element.querySelectorAll('strong'))
        .find(el => el.textContent.includes('Network'));
    const network = networkElement ? networkElement.nextSibling.textContent
    .trim().split(':').pop().trim() : null;

    var numericValue = convertToNumeric(formattedMarketCap);

    console.log("Network:", network);

    console.log('Monke PostRequest:', chatTitle, valueName, numericValue, valueAddress, network);
    sendPOSTRequest(valueName, numericValue, valueAddress, network, chatTitle);
}

function sendPOSTRequest(valueName, numericValue, valueAddress, network, chatTitle) {
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");

    const mkValue = isNaN(parseFloat(numericValue)) ? 0.0 : parseFloat(numericValue);

    const raw = JSON.stringify({
        "Name": valueName,
        "MK": mkValue,
        "Address": valueAddress,
        "Network": network,
        "ChatTitle": chatTitle
    });

    const requestOptions = {
        method: "POST",
        headers: myHeaders,
        body: raw,
        redirect: "follow"
    };

    // https://localhost:7111/data
    fetch("https://remsoftdev.dynamic-dns.net:82/data", requestOptions)
        .then((response) => {
            console.log('HTTP status:', response.status);
            return response.text();
        })
        .then((result) => console.log('Server response:', result))
        .catch((error) => console.error('Fetch error:', error));
}

// Function to scan all existing messages
function scanExistingMessages() {
    const messages = document.querySelectorAll('.translatable-message');
    
    console.log(new Date().toISOString().replace("T", " ").slice(0, 19));

    messages.forEach(message => {
        console.log("Existing message ID:", message.id);
    });
}

function scrollChatToBottom() {
    document.getElementsByClassName("bubbles-go-down")[0].click()

    const element = document.querySelector('.MessageList');
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}

if (typeof targetNode === "undefined") {
    // Select the node that you want to observe for changes
    const targetNode = document.querySelector('.chats-container');

    // Options for the observer (which mutations to observe)
    const config = { childList: true, subtree: true };

    // Callback function to execute when mutations are observed
    const callback = function(mutationsList, observer) {
        for (const mutation of mutationsList) {
            if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
                mutation.addedNodes.forEach(node => {
                    if (node instanceof HTMLElement && node.className && node.className.includes('bubbles-group')) {
                        try {
                            scrollChatToBottom();
                            // Parse the string as an HTML document
                            var parser = new DOMParser();
                            const chatTitleElement = document.querySelector('[class*="chat-info"] [class*="title"]');
                            const chatTitle = chatTitleElement?.textContent.trim();

                            var doc = parser.parseFromString(node.outerHTML, 'text/html');
                            var senderTitleElement = doc.querySelector('.translatable-message');

                            if(senderTitleElement){
                                if (node.outerText.startsWith('Rick'))
                                {
                                    logNewMessageRick(doc, chatTitle);
                                    console.log("Starting Procces: Rick");
                                }
                                else if (node.outerText.startsWith('MonkeBot'))
                                {
                                    logNewMessageMonke(doc, chatTitle);
                                    console.log("Starting Procces: MonkeBot");        
                                }
                                else
                                {
                                    logNewMessagePhanes(doc, chatTitle);
                                    console.log("Starting Procces: Phanes");
                                }
                            }
                            else{
                                var baseText = node.innerText;

                                const ethRegex = /0x[a-fA-F0-9]{40}/;
                                const solRegex = /[1-9A-HJ-NP-Za-km-z]{32,44}/;

                                let valueAddress = '';
                                let network = '';
                                let valueName = '';
                                let numericValue = '';

                                if (ethRegex.test(baseText)) {
                                    valueAddress = ethRegex.exec(baseText)[0];
                                    network = 'Ethereum';
                                } else if (solRegex.test(baseText)) {
                                    valueAddress = solRegex.exec(baseText)[0];
                                    network = 'Solana';
                                }

                                if (valueAddress && network) {
                                    var raw = JSON.stringify({
                                        "Address": valueAddress,
                                        "Network": network
                                    });

                                    console.log("Extracted:", raw);

                                    sendPOSTRequest(valueName, numericValue, valueAddress, network, chatTitle);
                                }
                            }
                        } catch (error) {
                            console.error("Error processing node:", error);
                        }
                    }
                });
            }
        }
        scrollChatToBottom();
    };
    // Create an observer instance linked to the callback function
    const observer = new MutationObserver(callback);

    // Start observing the target node for configured mutations
    if (targetNode) {
        observer.observe(targetNode, config);
        // Scan existing messages when the script is first run
        scanExistingMessages();
    } else {
        console.error("Target node not found.");
    }
}
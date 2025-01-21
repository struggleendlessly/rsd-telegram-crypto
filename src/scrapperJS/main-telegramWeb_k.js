// Function to log "trigger" to the console when a new message appears
function logNewMessagePhanes(doc) {
    var element = doc.querySelector('.translatable-message');
    
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

    console.log('Phanes PostRequest:', valueName, numericValue, valueAddress, network);
    sendPOSTRequest(valueName, numericValue, valueAddress, network);
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

function logNewMessageRick(doc) {
    var element = doc.querySelector('.translatable-message');

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

    var targetImgValue = doc.querySelector('img.emoji.emoji-image[alt="💎"]');

    var formattedMarketCap = "";

    if (targetImgValue) {
        var formattedMarketCap = targetImgValue.nextElementSibling.textContent.trim();

        console.log("Extracted address:", formattedMarketCap);
    } else {
        console.log("Target <img> not found");
    }

    var numericValue = convertToNumeric(formattedMarketCap);

    var targetImgNetwork = doc.querySelector('img.emoji.emoji-image[alt="🌐"]');

    var network = "";

    if (targetImgNetwork) {
        var network = targetImgNetwork.nextSibling.textContent.trim();
    
        console.log("Network:", network);
    } else {
        console.log("Target <img> not found");
    }

    console.log('Rick PostRequest:', valueName, numericValue, valueAddress, network);
    sendPOSTRequest(valueName, numericValue, valueAddress, network);
}

function logNewMessageMonke(doc) {
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

    console.log('Monke PostRequest:', valueName, numericValue, valueAddress, network);
    sendPOSTRequest(valueName, numericValue, valueAddress, network);
}

function sendPOSTRequest(valueName, numericValue, valueAddress, network) {
    
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");
    
    const raw = JSON.stringify({
      "Name": valueName,
      "MK": numericValue,
      "Address": valueAddress,
      "Network": network
    });
    
    const requestOptions = {
      method: "POST",
      headers: myHeaders,
      body: raw,
      redirect: "follow"
    };
    
    fetch("https://188.239.185.18:82/data", requestOptions)
      .then((response) => response.text())
      .then((result) => console.log(result))
      .catch((error) => console.error(error));
}
 
// Function to scan all existing messages
function scanExistingMessages() {
    const messages = document.querySelectorAll('.translatable-message');
    messages.forEach(message => {
        console.log("Existing message ID:", message.id);
    });
}
 
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
                        var doc = parser.parseFromString(node.outerHTML, 'text/html');
                        var senderTitleElement = doc.querySelector('.translatable-message');
 
                        if(senderTitleElement){
                            if (node.textContent.startsWith('Phanes')) {
                                logNewMessagePhanes(doc);
                                console.log("Starting Procces: Phanes");
                            }
                            if (node.textContent.startsWith('Rick'))
                            {
                                logNewMessageRick(doc);  
                                console.log("Starting Procces: Rick");
                            }
                            if (node.textContent.startsWith('MonkeBot'))
                            {
                                logNewMessageMonke(doc);
                                console.log("Starting Procces: MonkeBot");        
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
 
function scrollChatToBottom() {
    document.getElementsByClassName("bubbles-go-down")[0].click()
 
    const element = document.querySelector('.MessageList');
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
}
 
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
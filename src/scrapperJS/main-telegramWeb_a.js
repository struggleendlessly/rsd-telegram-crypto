// Function to log "trigger" to the console when a new message appears
function logNewMessagePhanes(doc) {
    var address = doc.querySelector('[data-entity-type="MessageEntityCode"]');
    var mk = doc.querySelector('[data-entity-type="MessageEntityUnderline"]');

    if (address && mk) {
        console.log('Extracted bot:', 'Phanes');

        var valueAddress = address.textContent.trim();
        console.log('Extracted address:', valueAddress);

        var valueMk = mk.textContent.trim();
        console.log('Extracted MK:', valueMk);

        var elementsName = doc.querySelectorAll('[data-entity-type="MessageEntityUrl"]');

        var secondElementName = elementsName[1];

        var canvas = doc.querySelector('[data-entity-type="MessageEntityUrl"]');
        if (canvas) {
            if (elementsName[0] && elementsName[0].textContent.trim()) {
                secondElementName = elementsName[0];
            }
        }

        if (secondElementName) {
            var valueName = "";

            var strongElementName = secondElementName.querySelector('strong');
            
            if (strongElementName) {
                valueName = strongElementName.textContent.trim();
            } else {
                valueName = secondElementName;
            }

            console.log('Extracted name:', valueName);

            var networkElement = doc.querySelectorAll('[data-entity-type="MessageEntityHashtag"]');

            var network = networkElement[0].textContent.trim().replace(/^#/, '');
            console.log('Network:', network);

            var numericValue = convertToNumeric(valueMk);

            console.log('Phanes PostRequest:', valueName, numericValue, valueAddress, network);
            sendPOSTRequest(valueName, numericValue, valueAddress, network);
        } else {
            console.error('No second element found in elementsName.');
        }
    }
}


function convertToNumeric(valueMk) {
    var currencyString = valueMk; // or "$686.9M"
    var multiplier = 1;

    if (currencyString.includes('K')) {
        multiplier = 1000;
    } else if (currencyString.includes('M')) {
        multiplier = 1000000;
    }

    var numericValue = parseFloat(currencyString.replace('$', '').replace(/[KM]/, '')) * multiplier;
    console.log(numericValue);
    return numericValue;    
    }

function logNewMessageRick(doc) {
    var element = doc.querySelector('.sender-title');
    
    var value = element.textContent.trim();
    console.log('Extracted bot:', value);
    
    //var valueAddress = doc.querySelector('.embedded-text-wrapper').querySelector('span').textContent.trim();
    var elements = doc.querySelectorAll('[data-entity-type="MessageEntityCode"]')
    var valueAddress = elements[elements.length -1].textContent.trim();
    console.log('Extracted address:', valueAddress);
    
    var valueMk = elements[1].textContent.trim();
    console.log('Extracted MK:', valueMk);
    
    var elementsName = doc.querySelectorAll('[data-entity-type="MessageEntityUrl"]');
    var secondElementName = elementsName[1];
    
    var strongElementName = secondElementName.querySelector('strong');
    var valueName = strongElementName.textContent.trim()
    console.log('Extracted name:', valueName);
    
    var targetImg = doc.querySelector('img.emoji.emoji-small[alt="🌐"]');

    var network = "";

    if (targetImg) {
        var nextNode = targetImg.nextSibling;
    
        while (nextNode && nextNode.nodeType !== Node.TEXT_NODE) {
            nextNode = nextNode.nextSibling;
        }
    
        if (nextNode) {
            network = nextNode.textContent.trim();
        }
    
        console.log("Network:", network);
    } else {
        console.log("Target <img> not found");
    }

    var numericValue = convertToNumeric(valueMk);

    console.log('Rick PostRequest:', valueName, numericValue, valueAddress, network);
    sendPOSTRequest(valueName, numericValue, valueAddress, network);
}

function logNewMessageMonke(doc) {
    var element = doc.querySelector('.sender-title');

    var value = element.textContent.trim();
    console.log('Extracted bot:', value);

    var elements = doc.querySelectorAll('[data-entity-type="MessageEntityCode"]');
    var valueAddress = elements[elements.length - 1].textContent.trim();
    console.log('Extracted address:', valueAddress);

    var marketCapElement = Array.from(doc.querySelectorAll('div'))
        .find(div => div.textContent.includes('Market Cap (FDV):'));
    var marketCapRaw = marketCapElement ? marketCapElement.textContent.split('Market Cap (FDV):')[1].trim() : 'Not found';

    var nameMatch = Array.from(doc.querySelectorAll('div'))
        .find(div => div.textContent.includes('MonkeBotadmin'));
    value = nameMatch ? nameMatch.textContent.split('MonkeBotadmin')[1].trim() : 'Not found';
    var valueNameMatch = value.match(/^(.+?)\s*-\s*/);
    var valueName = valueNameMatch ? valueNameMatch[1].trim() : 'Not found';

    var marketCapMatch = marketCapRaw.match(/\$[0-9,.]+/);
    var marketCap = marketCapMatch ? marketCapMatch[0] : 'Not found';

    var marketCapValue = parseFloat(marketCap.replace(/[^\d.]/g, ''));
    var marketCapInMillions = marketCapValue / 1_000_000;
    var formattedMarketCap = `$${marketCapInMillions.toFixed(2)}M`;
    console.log('Extracted MK:', formattedMarketCap);

    console.log('Extracted name:', valueName);

    var networkMatch = value.match(/Network:\s*(.+?)\s*(Price|$)/);
    var network = networkMatch ? networkMatch[1].trim() : 'Not found';
    console.log('Network:', network);

    var numericValue = convertToNumeric(formattedMarketCap);

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
    const messages = document.querySelectorAll('[id^="message-"]');
    messages.forEach(message => {
        console.log("Existing message ID:", message.id);
    });
}
 
// Select the node that you want to observe for changes
const targetNode = document.querySelector('.messages-container');
 
// Options for the observer (which mutations to observe)
const config = { childList: true, subtree: true };
 
// Callback function to execute when mutations are observed
const callback = function(mutationsList, observer) {
    for (const mutation of mutationsList) {
        if (mutation.type === 'childList' && mutation.addedNodes.length > 0) {
            mutation.addedNodes.forEach(node => {
                if (node.id && node.id.startsWith("message-")) {
                    try {
                        // Parse the string as an HTML document
                        var parser = new DOMParser();
                        var doc = parser.parseFromString(node.outerHTML, 'text/html');
                        var senderTitleElement = doc.querySelector('.sender-title');
                        var value = "Phanes";

                        if (senderTitleElement) {
                            value = senderTitleElement.textContent.trim();
                        }
                        if (value.includes("Phanes")) {
                            logNewMessagePhanes(doc);
                        }
                        if (value.includes("Rick"))
                        {
                            logNewMessageRick(doc);  
                        }
                        if (value.includes("MonkeBot"))
                        {
                            logNewMessageMonke(doc);  
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
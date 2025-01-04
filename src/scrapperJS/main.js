// Function to log "trigger" to the console when a new message appears
function logNewMessagePhanes(doc) {
    var address = doc.querySelector('[data-entity-type="MessageEntityCode"]');
    var valueAddress = address.textContent.trim();
    console.log('Extracted address:', valueAddress);

    var mk = doc.querySelector('[data-entity-type="MessageEntityUnderline"]');
    var valueMk = mk.textContent.trim();
    console.log('Extracted MK:', valueMk);

    var elementsName = doc.querySelectorAll('[data-entity-type="MessageEntityUrl"]');
    var secondElementName = elementsName[1];
    var canvas = doc.querySelector('[data-entity-type="MessageEntityUrl"]');
    if (canvas)
        var secondElementName = elementsName[0];
    
    var strongElementName = secondElementName.querySelector('strong');
    var valuenName = strongElementName.textContent.trim()
    console.log('Extracted name:', valuenName);

    var numericValue = convertToNumeric(valueMk);

    sendPOSTRequest(valuenName, numericValue, valueAddress);
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
    var parser = new DOMParser();
    var doc = parser.parseFromString(yourHtmlString, 'text/html');
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
    var valuenName = strongElementName.textContent.trim()
    console.log('Extracted name:', valuenName);

    var numericValue = convertToNumeric(valueMk);
    
    sendPOSTRequest(valuenName, numericValue, valueAddress);
}

function sendPOSTRequest(valueName, numericValue, valueAddress) {
    
    const myHeaders = new Headers();
    myHeaders.append("Content-Type", "application/json");
    
    const raw = JSON.stringify({
      "Name": valueName,
      "MK": numericValue,
      "Address": valueAddress
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
                    //console.log(node.outerHTML);
                    var parser = new DOMParser(); // Parse the string as an HTML document 
                    var doc = parser.parseFromString(node.outerHTML, 'text/html');
                    var value = doc.querySelector('.sender-title').textContent.trim();
                    if (value.includes("Phanes"))
                    {
                        logNewMessagePhanes(doc);  
                    }
                    
                    if (value.includes("Rick"))
                    {
                        logNewMessageRick(doc);  
                    }
                }
            });
        }
    }
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
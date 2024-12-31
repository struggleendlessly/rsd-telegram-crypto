// CSS class names used to identify key elements in the DOM
const messagesContainerClass = 'messages-container'; // Container holding all message groups
const messageDateGroupClass = 'message-date-group';  // Individual message group
const messageClass = 'message-list-item';            // Individual message item
const apiUrl = 'https://188.239.185.18:82/data';      // API endpoint for data submission

// Array of usernames to process.
// Example: ['User1', 'User2', 'User3']
const usernamesToProcess = ['Defined Bot', 'Proficy Price Bot', 'MonkeBot', 'Rick', 'Phanes [Gold]'];

// Styles for log messages
const successLogStyle = 'color: green;';
const infoLogStyle = 'color: blue;';
const warningLogStyle = 'color: orange;';

/**
 * Represents the data sent to the API.
 */
class ApiMessageData {
    /**
     * Constructs a new ApiMessageData instance.
     * @param {string} name - The Name field value.
     * @param {number|string} mk - The MK field value. If a string, it will be parsed as a number.
     * @param {string} address - The Address field value.
     */
    constructor(name, mk, address) {
        this.Name = name || 'Unknown'; // Default to 'Unknown' if no name is provided

        // Ensure MK is a number. If parsing fails, default to 0.
        const parsedMk = Number(mk);
        this.MK = isNaN(parsedMk) ? 0 : parsedMk;

        this.Address = address || 'Unknown';  // Default to 'Unknown' if no address is provided
    }

    /**
     * Validates the data object.
     * @returns {boolean} - Whether the object is valid.
     */
    isValid() {
        return Boolean(this.Name && this.MK > 0 && this.Address);
    }

    /**
     * Converts the object to JSON.
     * @returns {string} - JSON representation of the object.
     */
    toJSON() {
        return JSON.stringify({
            Name: this.Name,
            MK: this.MK,
            Address: this.Address,
        });
    }
}

/**
 * Sends data to the specified API endpoint.
 * @param {ApiMessageData} messageData - The data to send to the API.
 */
async function sendDataToApi(messageData) {
    if (!messageData.isValid()) {
        console.warn(`%cInvalid message data: ${messageData.toJSON()}`, warningLogStyle);
        return;
    }

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: messageData.toJSON(),
        });

        if (response.ok) {
            const currentTime = new Date().toLocaleString();
            console.log(`%cMessage sent successfully at ${currentTime}`, successLogStyle);
        } else {
            console.error('Failed to send data: ', response.status, response.statusText);
        }
    } catch (error) {
        console.error('Error during API call: ', error);
    }
}

/**
 * Extraction logic for Defined Bot.
 * @param {HTMLElement} messageNode - The message DOM element.
 * @returns {ApiMessageData} - Extracted data as an ApiMessageData instance.
 */
function extractForDefinedBot(messageNode) {
    const messageTextContentNode = messageNode
            .querySelector('.message-content-wrapper')
            ?.querySelector('.message-content')
            ?.querySelector('.content-inner')
            ?.querySelector('.text-content');

    // Get the name from the first <a> tag
    const firstLink = messageTextContentNode.querySelector('a');
    const linkText = firstLink ? firstLink.textContent.trim() : null;

    // Get the MK from the first <code> tag
    const firstCodeBlock = messageTextContentNode.querySelector('code');
    const codeText = firstCodeBlock ? firstCodeBlock.textContent.trim() : null;

    // Find the text containing "Mkt. Cap (FDV):" and extract the address after the dollar sign
    const mktCapMatch = messageTextContentNode.textContent.match(/Mkt\. Cap \(FDV\):\s*\$([\d,]+)/);
    const mktCapText = mktCapMatch ? mktCapMatch[1].replace(/,/g, '') : null;

    // Return the extracted data as an object
    return new ApiMessageData(linkText, mktCapText, codeText);
}

/**
 * Extraction logic for Proficy Price Bot.
 * @param {HTMLElement} messageNode - The message DOM element.
 * @returns {ApiMessageData} - Extracted data as an ApiMessageData instance.
 */
function extractForProficyPriceBot(messageNode) {
    const name = messageNode.querySelector('.username')?.textContent.trim() || null;
    const mk = messageNode.querySelector('.info .mk-field')?.textContent.trim() || null;
    const address = messageNode.querySelector('.info .address-field')?.textContent.trim() || null;
    return new ApiMessageData(name, mk, address);
}

/**
 * Extraction logic for MonkeBot.
 * @param {HTMLElement} messageNode - The message DOM element.
 * @returns {ApiMessageData} - Extracted data as an ApiMessageData instance.
 */
function extractForMonkeBot(messageNode) {
    const name = messageNode.querySelector('.username')?.textContent.trim() || null;
    const mk = messageNode.querySelector('.info .mk-field')?.textContent.trim() || null;
    const address = messageNode.querySelector('.info .address-field')?.textContent.trim() || null;
    return new ApiMessageData(name, mk, address);
}

/**
 * Extraction logic for Rick.
 * @param {HTMLElement} messageNode - The message DOM element.
 * @returns {ApiMessageData} - Extracted data as an ApiMessageData instance.
 */
function extractForRick(messageNode) {
    const name = messageNode.querySelector('.username')?.textContent.trim() || null;
    const mk = messageNode.querySelector('.info .mk-field')?.textContent.trim() || null;
    const address = messageNode.querySelector('.info .address-field')?.textContent.trim() || null;
    return new ApiMessageData(name, mk, address);
}

/**
 * Extraction logic for Phanes [Gold].
 * @param {HTMLElement} messageNode - The message DOM element.
 * @returns {ApiMessageData} - Extracted data as an ApiMessageData instance.
 */
function extractForPhanesGold(messageNode) {
    const name = messageNode.querySelector('.username')?.textContent.trim() || null;
    const mk = messageNode.querySelector('.info .mk-field')?.textContent.trim() || null;
    const address = messageNode.querySelector('.info .address-field')?.textContent.trim() || null;
    return new ApiMessageData(name, mk, address);
}

// Mapping of usernames to their respective extraction methods
const senderExtractionMethods = {
    'Defined Bot': extractForDefinedBot,
    'Proficy Price Bot': extractForProficyPriceBot,
    'MonkeBot': extractForMonkeBot,
    'Rick': extractForRick,
    'Phanes [Gold]': extractForPhanesGold
};

/**
 * Processes a newly added message node.
 * @param {HTMLElement} messageNode - The message DOM element.
 */
function handleNewMessage(messageNode) {
    try {
        // Locate the username within the message node
        const usernameElement = messageNode
            .querySelector('.message-content-wrapper')
            ?.querySelector('.message-content')
            ?.querySelector('.content-inner')
            ?.querySelector('.message-title')
            ?.querySelector('.sender-title');

        const username = usernameElement?.textContent.trim();

        // Check if the username is in the list to process
        if (usernamesToProcess.length > 0 && !usernamesToProcess.includes(username)) {
            return;
        }

        const extractData = senderExtractionMethods[username];
        if (!extractData) {
            console.warn(`%cNo extraction method defined for sender: ${username}`, warningLogStyle);
            return;
        }

        const messageData = extractData(messageNode);
        sendDataToApi(messageData);
    } catch (error) {
        console.error('Error while processing new message: ', error);
    }
}

/**
 * Monitors a specific message date group for new messages.
 * @param {HTMLElement} messageDateGroupNode - The message group DOM element.
 */
function monitorMessageDateGroup(messageDateGroupNode) {
    const observer = new MutationObserver((mutationsList) => {
        for (const mutation of mutationsList) {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(node => {
                    if (node.nodeType === 1 && node.classList.contains(messageClass)) {
                        handleNewMessage(node);
                    }
                });
            }
        }
    });

    observer.observe(messageDateGroupNode, {
        childList: true,
        subtree: false,
    });
}

/**
 * Processes existing message groups and monitors them for new messages.
 * @param {HTMLElement} messagesContainer - The container holding all message groups.
 */
function processExistingMessageGroups(messagesContainer) {
    const existingGroups = messagesContainer.querySelectorAll(`.${messageDateGroupClass}`);
    existingGroups.forEach(monitorMessageDateGroup);
}

/**
 * Monitors the messages container for new message groups.
 * @param {HTMLElement} messagesContainer - The container holding all message groups.
 */
function monitorMessageGroups(messagesContainer) {
    const observer = new MutationObserver((mutationsList) => {
        for (const mutation of mutationsList) {
            if (mutation.type === 'childList') {
                mutation.addedNodes.forEach(node => {
                    if (node.nodeType === 1 && node.classList.contains(messageDateGroupClass)) {
                        monitorMessageDateGroup(node);
                    }
                });
            }
        }
    });

    observer.observe(messagesContainer, {
        childList: true,
        subtree: false,
    });
}

// Locate the messages container and set up monitoring
const container = document.querySelector(`.${messagesContainerClass}`);
const messagesContainer = document.querySelector(`.${messagesContainerClass}`);
if (!messagesContainer) {
    console.error('Messages container not found!');
} else {
    processExistingMessageGroups(messagesContainer); // Process existing groups
    monitorMessageGroups(messagesContainer);         // Monitor for new groups

    const currentTime = new Date().toLocaleString();
    console.log(`%cMonitoring started at ${currentTime}`, infoLogStyle);
}

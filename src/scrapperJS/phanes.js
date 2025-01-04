
// Create a new DOMParser
var parser = new DOMParser();

// Parse the string as an HTML document
var doc = parser.parseFromString(yourHtmlString, 'text/html');

// Use querySelector to find the element
var element = doc.querySelector('.sender-title');


// Extract the value
var value = element.textContent.trim();
console.log('Extracted bot:', value);

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



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



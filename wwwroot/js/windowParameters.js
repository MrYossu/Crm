export function bringToFront(windowSelector) {
  let windowElement = document.querySelector(windowSelector);
  if (windowElement) {
    windowElement.focus();
  }
}
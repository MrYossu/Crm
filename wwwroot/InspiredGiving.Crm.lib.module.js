export function afterStarted(blazor) {
  blazor.registerCustomEventType("pastemedia", {
    browserEventName: 'paste',
    createEventArgs: event => {
      let isMultimedia = false;

      // We get the saved text in the clipboard
      let data = event.clipboardData.getData('text');

      const items = event.clipboardData.items;

      // With this we'll filter the file by its media type
      const acceptedMediaTypes = ['image/png'];

      for (let i = 0; i < items.length; i++) {
        const file = items[i].getAsFile();

        // We verify there's a file in the current item
        if (!file) {
          continue;
        }

        // We verify the media type of the file
        if (acceptedMediaTypes.indexOf(items[i].type) === -1) {
          continue;
        }

        // it's an image
        isMultimedia = true;
        const url = window.URL || window.webkitURL;
        data = url.createObjectURL(file);
      }

      // We are passing the information to the custom event from JavaScript
      return {
        isMultimedia,
        data
      }
    }
  });
}
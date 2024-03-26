const blobAsBuffer = async blob => {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result);
    reader.error = (err) => reject(err);
    reader.readAsArrayBuffer(blob);
  })
}

window.convertBlobURLToBase64 = async (url) => {
  const response = await fetch(url);
  //console.log("response");
  //console.log(response);
  const blob = await response.blob();
  //console.log("blob");
  //console.log(blob);
  const buffer = await blobAsBuffer(blob);
  return buffer;
}

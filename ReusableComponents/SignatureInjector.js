function (e,v) {
	try {
		const obj = JSON.parse(v);
		var b64     = obj.SignatureItem;                         
		var canvas  = document.getElementById(obj.CanvasId);
		var ctx     = canvas.getContext('2d');

		var img = new Image();
        img.onload = function () {
        ctx.clearRect(0, 0, canvas.width, canvas.height); 
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);
        };
        img.src = 'data:image/png;base64,' + b64;
		return `success`;
	} catch (error) {
		return `failed with error:${error}`;
	}
};
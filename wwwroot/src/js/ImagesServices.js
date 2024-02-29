import {uuidv4} from './utilities.js';
import { fabric } from "fabric";

import { ucFirst, findById } from './utilities.js';
import { default as fabCanvas } from './fabricCanvas.js';

function makeFromDocumentBodyDropImageZone(hubConnection) {

    const dropZone = document.body;

    if (dropZone) {
        //let hoverClassName = 'hover';

        dropZone.addEventListener("dragenter", function (e) {
            e.preventDefault();
            //dropZone.classList.add(hoverClassName);
        });

        dropZone.addEventListener("dragover", function (e) {
            e.preventDefault();
            //dropZone.classList.add(hoverClassName);
        });

        dropZone.addEventListener("dragleave", function (e) {
            e.preventDefault();
            //dropZone.classList.remove(hoverClassName);
        });

        dropZone.addEventListener("drop", function (e) {
            e.preventDefault();
            //dropZone.classList.remove(hoverClassName);

            const files = Array.from(e.dataTransfer.files);

            if (!FileReader || files.length <= 0)
                return

            let file = files[0];
            let scale = 1;
            let left = e.pageX - e.target.offsetLeft;
            let top = e.pageY - e.target.offsetTop;


            var fileReader = new FileReader();

            fileReader.onload = () => {

                let id = uuidv4()

                hubConnection.invoke("AddImage", { "Id": id, "Data": fileReader.result, "Scale": scale, "Left": left, "Top": top, })
                    .catch(function (err) {
                        return console.error(err.toString());
                    });
            }

            fileReader.readAsDataURL(file);
        });
    }
}

function AddImageManipulations(hubConnection) {

    makeFromDocumentBodyDropImageZone(hubConnection);

    hubConnection.on("AddImageOnCanvas", (img) => {

        let id = img.id;
        let data = img.data;
        let scale = img.scale;
        let left = img.left;
        let top = img.top;

        fabric.util.loadImage(data, (img) => {
            var oImg = new fabric.Image(img);

            oImg.scale(scale).set({
                id: id,
                left: left,
                top: top,
            });

            // oImg.on('moving', function () {
            //     console.log('selected a rectangle');
            // });

            fabCanvas.add(oImg);
            fabCanvas.renderAll();
        });
    })

    hubConnection.on("MoveObjectOnCanvas", (payload) => {

        let id = payload.id;
        let left = payload.left;
        let top = payload.top;

        let obj = findById(fabCanvas, id);

        if (!obj) return;

        obj.set({ left: left, top: top });

        fabCanvas.renderAll();
    })

    hubConnection.on("ScaleObjectOnCanvas", (payload) => {

        let id = payload.id;
        let left = payload.left;
        let top = payload.top;
        let width = payload.width;
        let height = payload.height;
        let scaleX = payload.scaleX;
        let scaleY = payload.scaleY;        

        let obj = findById(fabCanvas, id);

        if (!obj) return;

        obj.set({ left: left, top: top, width: width, height: height, scaleX: scaleX, scaleY: scaleY });

        fabCanvas.renderAll();
    })

    hubConnection.start()
        .catch(function (err) {
            return console.error(err.toString());
        });

    fabCanvas.on('object:modified', (evt) => {

        let method = ucFirst(evt.action);

        var modifiedObject = evt.target;
        let id = modifiedObject.get('id');
        let left = modifiedObject.get('left');
        let top = modifiedObject.get('top');
        let width = modifiedObject.get('width');
        let height = modifiedObject.get('height');
        let scaleX = modifiedObject.get('scaleX');
        let scaleY = modifiedObject.get('scaleY');

        let payload;

        switch (method) {
            case 'Drag':
                payload = { "Id": id, "Left": left, "Top": top, };
                break
            case 'Scale':
                payload = { "Id": id, "Left": left, "Top": top, "Width": width, "Height": height, "ScaleX": scaleX, "ScaleY": scaleY, };
                break
            default:
                return
        }

        hubConnection.invoke(method, payload)
            .catch(function (err) {
                return console.error(err.toString());
            });
    });
}

export {AddImageManipulations};
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
            let left = e.pageX - e.target.offsetLeft;
            let top = e.pageY - e.target.offsetTop;
            let scaleX = 1;
            let scaleY = 1;

            var fileReader = new FileReader();

            fileReader.onload = () => {

                let id = uuidv4()

                hubConnection.invoke("AddObject", { 
                    "Id": id, 
                    "Data": fileReader.result, 
                    "Left": left, 
                    "Top": top,
                    "ScaleX": scaleX,
                    "ScaleY": scaleY,
                })
                .catch(function (err) {
                    return console.error(err.toString());
                });
            }

            fileReader.readAsDataURL(file);
        });
    }
}

function getAllDrawingObjectsFromBackend(hubConnection) {
    hubConnection.invoke('GetAllDrawingObjects')
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function addImageManipulations(hubConnection) {

    makeFromDocumentBodyDropImageZone(hubConnection);
    getAllDrawingObjectsFromBackend(hubConnection);

    hubConnection.on("AddObjectOnCanvas", (img) => {

        let id = img.id;
        let data = img.data;
        let left = img.left;
        let top = img.top;
        let scaleX = img.scaleX;
        let scaleY = img.scaleY;

        fabric.util.loadImage(data, (img) => {
            var oImg = new fabric.Image(img);

            oImg.set({
                id: id,
                left: left,
                top: top,
                scaleX: scaleX,
                scaleY: scaleY,
            });

            fabCanvas.add(oImg);
            fabCanvas.renderAll();
        });
    })

    hubConnection.on("AddObjectError", (img) => {
        //Add error handler!!!
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
        let scaleX = payload.scaleX;
        let scaleY = payload.scaleY;        

        let obj = findById(fabCanvas, id);

        if (!obj) return;

        obj.set({ left: left, top: top, scaleX: scaleX, scaleY: scaleY });

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
        let scaleX = modifiedObject.get('scaleX');
        let scaleY = modifiedObject.get('scaleY');

        let payload;

        switch (method) {
            case 'Drag':
                payload = { "Id": id, "Left": left, "Top": top, };
                break
            case 'Scale':
            case 'ScaleX':
            case 'ScaleY':
                method = "Scale"
                payload = { "Id": id, "Left": left, "Top": top, "ScaleX": scaleX, "ScaleY": scaleY, };
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

export {addImageManipulations};
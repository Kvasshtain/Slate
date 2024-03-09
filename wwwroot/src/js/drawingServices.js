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

            let fileReader = new FileReader();

            fileReader.onload = () => {

                fabric.util.loadImage(fileReader.result, (img) => {
                    let oImg = new fabric.Image(img);
        
                    oImg.set({
                        left: left,
                        top: top,
                    });
        
                    fabCanvas.add(oImg);
                    fabCanvas.renderAll();
                });
            }

            fileReader.readAsDataURL(file);
        });
    }
}

function getAllBoardObjectsFromBackend(hubConnection) {
    hubConnection.invoke('GetAllBoardObjects')
        .catch(function (err) {
            return console.error(err.toString());
        });
}

function addImageManipulations(hubConnection) {

    makeFromDocumentBodyDropImageZone(hubConnection);
    getAllBoardObjectsFromBackend(hubConnection);

    hubConnection.on("AddObjectError", (img) => {
        //Add error handler!!!
    })

    hubConnection.on("AddObjectOnCanvas", (obj) => {
        let id = obj.id;
        let canvasObj = JSON.parse(obj.data);
        let left = obj.left;
        let top = obj.top;
        let scaleX = obj.scaleX;
        let scaleY = obj.scaleY;
        let angle = obj.angle;

        fabric.util.enlivenObjects([canvasObj], function (objects) {
            var origRenderOnAddRemove = fabCanvas.renderOnAddRemove;
            fabCanvas.renderOnAddRemove = false;

            objects.forEach(function (o) {
                o.set({
                    id: id,
                    left: left,
                    top: top,
                    scaleX: scaleX,
                    scaleY: scaleY,
                    angle: angle,
                });

                fabCanvas.add(o);
            });

            fabCanvas.renderOnAddRemove = origRenderOnAddRemove;
            fabCanvas.renderAll();
        });

        fabCanvas.renderAll();
    })

    hubConnection.on("MoveObjectOnCanvas", (payload) => {

        let id = payload.id;
        let left = payload.left;
        let top = payload.top;

        let obj = findById(fabCanvas, id);

        if (!obj) return;

        obj.set({ 
            left: left, 
            top: top, 
        });

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

        obj.set({ 
            left: left, 
            top: top, 
            scaleX: scaleX, 
            scaleY: scaleY, 
        });

        fabCanvas.renderAll();
    })

    hubConnection.on("RotateObjectOnCanvas", (payload) => {

        let id = payload.id;
        let angle = payload.angle;      

        let obj = findById(fabCanvas, id);

        if (!obj) return;

        obj.set({ 
            angle: angle, 
        });

        fabCanvas.renderAll();
    })

    hubConnection.start()
        .catch(function (err) {
            return console.error(err.toString());
        });

    fabCanvas.on('object:added', (evt) => {

        let target = evt.target;

        if (target.id === undefined) {
            let id = uuidv4();
            let left = target.left;
            let top = target.top;
            let scaleX = target.scaleX;
            let scaleY = target.scaleY;

            let jsonData = JSON.stringify(target);

            fabCanvas.remove(target);

            hubConnection.invoke("AddObject", {
                "Id": id,
                "Data": jsonData,
                "Left": left, 
                "Top": top,
                "ScaleX": scaleX,
                "ScaleY": scaleY,
            })
                .catch(function (err) {
                    return console.error(err.toString());
                });

            fabCanvas.isDrawingMode = false; //!!!!
        }
        //fabCanvas.isDrawingMode = false; //!!!!
    })

    fabCanvas.on('object:modified', (evt) => {

        let method = ucFirst(evt.action);

        var modifiedObject = evt.target;
        let id = modifiedObject.get('id');
        let left = modifiedObject.get('left');
        let top = modifiedObject.get('top');
        let scaleX = modifiedObject.get('scaleX');
        let scaleY = modifiedObject.get('scaleY');
        let angle = modifiedObject.get('angle');

        let payload;

        switch (method) {
            case 'Drag':               
                payload = { 
                    "Id": id, 
                    "Left": left, 
                    "Top": top, 
                };
                break
            case 'Scale':
            case 'ScaleX':
            case 'ScaleY':
                method = "Scale"                
                payload = { 
                    "Id": id, 
                    "Left": left, 
                    "Top": top, 
                    "ScaleX": scaleX, 
                    "ScaleY": scaleY, 
                };
                break
            case 'Rotate':              
                payload = { 
                    "Id": id, 
                    "Angle": angle, 
                };
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
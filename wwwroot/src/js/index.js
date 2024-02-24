

window.addEventListener('load', (event) => {

    //Utilities
    function uuidv4() {
        return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, c =>
            (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
        );
    }                                                                                                        

    function ucFirst(str) {
        if (!str) return str;

        return str[0].toUpperCase() + str.slice(1);
    }

    function findById(canvas, id) {
        return canvas.getObjects().find(obj => obj.id === id);
    }
    //Utilities

    const hubConnection = new signalR.HubConnectionBuilder()
        .withUrl("/imageExchanging")
        .configureLogging(signalR.LogLevel.Information)
        .build();

    hubConnection.on("AddImageOnCanvas", (img) => {

        let id = img.id;
        let data = img.data;
        let scale = img.scale;
        let left = img.left;
        let top = img.top

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

            canvas.add(oImg);
        });
    })

    hubConnection.on("MoveObjectOnCanvas", (payload) => {

        let id = payload.id;
        let left = payload.left;
        let top = payload.top;

        let obj = findById(canvas, id);

        if (!obj) return;

        obj.set({ left: left, top: top });

        canvas.renderAll();
    })

    hubConnection.start()
        .catch(function (err) {
            return console.error(err.toString());
        });

    var canvas = new fabric.Canvas('canvas')




    //events
    var modifiedHandler = function (evt) {

        let method = ucFirst(evt.action);

        var modifiedObject = evt.target;
        let id = modifiedObject.get('id');
        let left = modifiedObject.get('left');
        let top = modifiedObject.get('top');

        let payload;

        switch (method) {
            case 'Drag':
                payload = { "Id": id, "Left": left, "Top": top, }
                break
            default:
                return
                break
        }

        hubConnection.invoke(method, payload)
            .catch(function (err) {
                return console.error(err.toString());
            });

    };

    canvas.on('object:modified', modifiedHandler);
    //events




    

    const dropZone = document.body;
    //var dropZone = document.getElementById('canvas');
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

            let file = files[0]
            let scale = 1
            let left = 100
            let top = 100


            var fileReader = new FileReader();

            fileReader.onload = () => {

                // fabric.util.loadImage(fileReader.result, (img) => {
                //     var oImg = new fabric.Image(img);
                //     oImg.scale(1).set({
                //         left: 100,
                //         top: 100,
                //     });
                //     canvas.add(oImg);
                // });

                let id = uuidv4()

                hubConnection.invoke("AddImage", { "Id": id, "Data": fileReader.result, "Scale": scale, "Left": left, "Top": top, })
                    .catch(function (err) {
                        return console.error(err.toString());
                    });
            }

            fileReader.readAsDataURL(file);
        });
    }
})
var canvas;
var ctx;
var image;
var fr;
var scale = 3;
var data = [];

window.onload = function() {
    canvas=document.getElementById("canvas");
    canvas.onclick = function(event){
        var rect = canvas.getBoundingClientRect();
        var xpos = event.clientX - rect.left;
        var ypos = event.clientY - rect.top;
        console.log("x" + xpos + " y" + ypos);

        var i=toGrid(ypos);
        var j=toGrid(xpos);
        var k=whichTri(toLocal(xpos), toLocal(ypos));
        data[i][j][k]=(data[i][j][k]+1)%5;
        redraw();
    }

    ctx=canvas.getContext("2d");
    ctx.imageSmoothingEnabled = false;
    ctx.scale(scale,scale);

    image = new Image();
    image.onload = function () { 
        buildData();
        redraw();
    }

    fr = new FileReader();
    fr.onload = function () {
        image.src = fr.result
    }
}

function onFileSelected(event) {
    var file = event.target.files[0];
    if (file) fr.readAsDataURL(file);
}


function buildData(){
    data = [];
    for (var i=0; i*20<image.height; i++){
        data[i]=[];
        for (var j=0; j*20<image.width; j++){
            data[i][j]=[];
            for (var k=0; k<4; k++){
                data[i][j][k]=0;
            }
        }
    }
}

function printData(){
    var result = "{";
    for (var i=0; i<data.length; i++){
        result += i==0?"\n{":",\n{";
        for (var j=0; j<data.length; j++){
            result += j==0?"{":",{";
            result += data[i][j].toString();
            result += "}";
        }
        result += "}";
    }
    result += "\n}";
    return result;
}

function selectTri(){}

function setData(i,j,k,value){
    if (!data[i]) data[i] = [];
    if (!data[i][j]) data[i][j] = [];

    data[i][j][k]=value;
}

function getData(i,j,k){
    if (!data[i] ||
        !data[i][j]) return;
    
    return data[i][j][k];
}

function redraw(){
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.drawImage(image, 0, 0);
    ctx.beginPath();
    ctx.rect(0,0,canvas.width,canvas.height);
    ctx.fillStyle = 'rgba(255,255,255,0.5)';
    ctx.fill();
    for (var i in data){
        for (var j in data[i]){
            for (var k=0; k<4; k++){
                //0-top 1-left 2-bottom 3-right
                var value=data[i][j][k];

                switch (value){
                    case 1: drawTri(i,j,k,'#000000'); break;
                    case 2: drawTri(i,j,k,'#ff0000'); break;
                    case 3: drawTri(i,j,k,'#00ff00'); break;
                    case 4: drawTri(i,j,k,'#0000ff'); break;
                }
            }
        }
    }
}

function drawTri(i,j,k,color){
    var ytri = toWorld(i);
    var xtri = toWorld(j);
    
    ctx.beginPath();
    switch(k){
        case 0:
            //top
            ctx.moveTo(xtri, ytri);
            ctx.lineTo(xtri + 20, ytri);
            ctx.lineTo(xtri + 10, ytri + 10);
            break;
        case 1:
            //left
            ctx.moveTo(xtri, ytri);
            ctx.lineTo(xtri + 10, ytri + 10);
            ctx.lineTo(xtri, ytri + 20);
            break;
        case 2:
            //bottom
            ctx.moveTo(xtri, ytri + 20);
            ctx.lineTo(xtri + 10, ytri + 10);
            ctx.lineTo(xtri + 20, ytri + 20);
            break;
        case 3:
            //right
            ctx.moveTo(xtri + 20, ytri);
            ctx.lineTo(xtri + 20, ytri + 20);
            ctx.lineTo(xtri + 10, ytri + 10);
            break;
    }
    ctx.closePath();
    ctx.strokeStyle = color;
    ctx.stroke()
}

function toWorld(z){ return z*20; }
function toGrid(z){ return Math.floor(z/20/scale); }
function toLocal(z){ return z/scale%20}

function whichTri(xLocal, yLocal) {
    var topleft = xLocal + yLocal < 20;
    var topright = (20 - xLocal) + yLocal < 20;

    if (topleft){
        if (topright) return 0; //top
        else return 1; //left
    }
    else{
        if (topright) return 3; //right
        else return 2; //bottom
    }
}
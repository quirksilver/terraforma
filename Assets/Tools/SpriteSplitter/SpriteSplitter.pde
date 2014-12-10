import controlP5.*;

ControlP5 cp5;

static final int SELECT_FILES = 0;
static final int SLICE_IMAGE = 1;

int currentMode = SELECT_FILES;

int footprintIndex = 0;

int imageX, imageY;
PVector imagePos;

String imgPath = "";
String xmlPath = "";
String saveLocation = "";

int isoOffset = 0;

PImage img;

int isoWidth = 134;
int isoHeight = 68;

PImage temp;

ArrayList<PImage> slicedSprites = new ArrayList<PImage>();

int pathsSet = 0;

PVector[] footprint;

Textlabel spriteFileLabel, xmlFileLabel, saveLocationLabel, errorLabel;
Textfield filenameInput;

void setup()
{
  size(1000, 500);
  background(100);
  hint(ENABLE_STROKE_PURE);

  rectMode(CORNERS);

  cp5 = new ControlP5(this);

  // create a new button with name 'buttonA'
  cp5.addButton("Load_Sprite_File")
    .setValue(0)
      .setPosition(10, 10)
        .setSize(100, 30)
          ;

  cp5.addButton("Load_Footprint_File")
    .setValue(0)
      .setPosition(10, 50)
        .setSize(100, 30)
          ;
          
  cp5.addButton("Choose_Save_Location")
  .setValue(0)
      .setPosition(10, 90)
        .setSize(100, 30)
          ;
          
  filenameInput = cp5.addTextfield("Filename")
     .setPosition(10,130)
     .setSize(200,20)
     .setFocus(true)
     .setColor(color(255,0,0))
     ;

  spriteFileLabel = cp5.addTextlabel("spriteFileLabel")
                    .setText("Not selected")
                    .setPosition(110,20)
                    .setColorValue(0xffffff00);
                    
  xmlFileLabel = cp5.addTextlabel("xmlFileLabel")
                    .setText("Not selected")
                    .setPosition(110,60)
                    .setColorValue(0xffffff00);
                    
  saveLocationLabel = cp5.addTextlabel("saveLocationLabel")
                    .setText("Not selected")
                    .setPosition(110,100)
                    .setColorValue(0xffffff00);
                    
   errorLabel = cp5.addTextlabel("errorLabel")
                    .setText("")
                    .setPosition(10,210)
                    .setColorValue(0xffff0000);
            
 cp5.addButton("Load_Sprite")
  .setValue(0)
      .setPosition(10, 170)
        .setSize(100, 30)
          ;





  /*img = loadImage("sprite1.png");
   
   img.loadPixels();
   
   int xNum = 0;
   int yNum = 0;
   
   //PVector[] footprint = new PVector[9];
   
   
   
   
   for (int i = 0; i < footprint.length; i++)
   {
   
   }*/
}

void draw()
{
  background(100);
  drawGrid();

  switch (currentMode)
  {
  case SLICE_IMAGE:
    drawImage();
    showSlice();
    drawCutoffLine();
    
    drawFootprint();
    break;
  }
}


public void controlEvent(ControlEvent theEvent) {
  println(theEvent.getController().getName());
}


public void Load_Sprite(int theValue)
{
  if (frameCount <= 1) return;
  
  if (imgPath != "" && xmlPath != "" && saveLocation != "" && filenameInput.getText() != "")
  {
    loadSprite();
    loadFootprint();

    getImageXY();
    currentMode = SLICE_IMAGE;
    
    errorLabel.setText("");
  }
  else
  {
    errorLabel.setText("Whoops, I can't do that, you've forgotten to set something.");
  }
  
  
  
}

public void Load_Sprite_File(int theValue) {
  if (frameCount >1) selectInput("Select the file for the sprite:", "spriteFileSelected");
}

void spriteFileSelected(File selection) {
  if (selection == null) {
    println("Window was closed or the user hit cancel.");
  } else {
    imgPath = selection.getAbsolutePath();
    spriteFileLabel.setText(imgPath);
    
    String name = selection.getName();
    
    filenameInput.setText(name.substring(0, name.length() - 4) + "Sheet");
    println("User selected " + selection.getAbsolutePath());
  }
}

public void Load_Footprint_File(int theValue) {
  if (frameCount >1) selectInput("Select the xml file for the footprint:", "footprintFileSelected");
}

void footprintFileSelected(File selection) {
  if (selection == null) {
    println("Window was closed or the user hit cancel.");
  } else {
    xmlPath = selection.getAbsolutePath();
    xmlFileLabel.setText(xmlPath);
    println("User selected " + selection.getAbsolutePath());
  }
}

public void Choose_Save_Location(int theValue) {
  if (frameCount >1) selectFolder("Select the location to save the files:", "saveLocationSelected");
}

void saveLocationSelected(File selection) {
  if (selection == null) {
    println("Window was closed or the user hit cancel.");
  } else {
    saveLocation = selection.getAbsolutePath() + "/";
    saveLocationLabel.setText(saveLocation);
    println("User selected " + selection.getAbsolutePath());
  }
}

void loadSprite()
{
  img = loadImage(imgPath);
}

void loadFootprint()
{
  XML footprintXML = loadXML(xmlPath);
  XML[] children = footprintXML.getChildren("tile");

  footprint = new PVector[children.length];

  float [] xValues = new float[footprint.length];

  for (int i = 0; i < children.length; i++) {

    footprint[i] = new PVector(children[i].getFloat("x"), children[i].getFloat("y"), children[i].getFloat("z"));

    xValues[i] = footprint[i].x - footprint[i].z;
  }

  getGridOffset(xValues);
}

void getGridOffset(float [] xValues)
{
  xValues = sort(xValues);

  isoOffset = (int)abs(xValues[0]);

  println("isoOffset " + isoOffset);
}

void drawGrid()
{
  for (int x = -isoWidth*5 - isoWidth/2; x <= width + isoWidth*5 + isoWidth/2; x+=isoWidth)
  {
    stroke(255);
    line(x, height, x + cos(radians(26.565))*1000, height - sin(radians(26.565))*1000);
    line(x, height, x + cos(radians(153.435))*1000, height - sin(radians(153.435))*1000);
  }
  
  noStroke();
  fill(100);
  rect(0,0, isoWidth*2, height);
}

void drawImage()
{
  if (img != null)
    tint(255, 128);

  imagePos = new PVector(isoWidth*4 - isoOffset*isoWidth/2, height - img.height); 

  image(img, imagePos.x, imagePos.y);

  //width/2 - isoOffset*isoWidth/2
}

void drawCutoffLine()
{

  stroke(135, 206, 250);
  line(0, mouseY, width, mouseY);
}

void mousePressed()
{
  if (currentMode == SLICE_IMAGE)
  {
    println("SLICE");
    sliceImage();
  }
}

void showSlice()
{
  noStroke();
  fill(135, 206, 250, 120);
  rect(imageX + imagePos.x, mouseY, imageX + imagePos.x + isoWidth, imageY + imagePos.y);
}

void sliceImage()
{
  int yCutoff = int(mouseY - imagePos.y);
  
  slicedSprites.add(img.get(imageX, yCutoff , isoWidth, imageY - yCutoff));

  //temp.save(saveLocation + filenameInput.getText() + nf((int)footprint[footprintIndex].x, 2) + nf((int)footprint[footprintIndex].z, 2) + ".png");

  nextSlice();
}

void nextSlice()
{
  if (footprintIndex < footprint.length - 1)
  {
    footprintIndex ++;

    getImageXY();
  } else
  {
    saveSpriteSheet();
    currentMode = SELECT_FILES;
  }
}

void saveSpriteSheet()
{
  int maxSpriteHeight = 0;
  
  for (int i = 0; i < slicedSprites.size(); i++)
  {
    if (slicedSprites.get(i).height > maxSpriteHeight)
    {
      maxSpriteHeight = slicedSprites.get(i).height;
    }
  }
  
  PImage spriteSheet = createImage(slicedSprites.size()*(isoWidth + 1), maxSpriteHeight, ARGB);
  
  for (int i = 0; i < slicedSprites.size(); i++)
  {
    PImage sprite = slicedSprites.get(i);
    spriteSheet.set(i*(isoWidth + 1), spriteSheet.height - sprite.height, sprite); 
  }
  
  spriteSheet.save(saveLocation + filenameInput.getText()+ ".png");
  
}

void getImageXY()
{
  imageX = round((footprint[footprintIndex].x - footprint[footprintIndex].z + 1)* isoWidth/2);
  imageY = round(img.height - (footprint[footprintIndex].x + footprint[footprintIndex].z)*isoHeight/2);
}

void drawFootprint()
{
  stroke(0);
  fill(255);
  pushMatrix();
  translate(50 + isoOffset*35, height- 50);
  rotate(-PI/4);
  for (int i = 0; i < footprint.length; i++)
  {
    if (i == footprintIndex)
    {
      fill(135, 206, 250);
    }
    else 
    {
      fill(255);
    }
    
    rect((footprint[i].x - 0.5) *50, -(footprint[i].z - 0.5)*50, (footprint[i].x + 0.5) *50, -(footprint[i].z + 0.5)*50);
  }
  
  popMatrix();
}


/*for (int x = 1; x <= img.width; x+= isoWidth/2)
 {
 for (int y = img.height; y > 0; y -= isoHeight)
 {
 //index = (y-1)*img.width + x-1;
 
 //temp = createImage(isoWidth, y, ARGB);
 
 temp = img.get(x + isoWidth/2, 0, isoWidth, y); 
 
 temp.save("temp" + nf(xNum, 2) + nf(yNum, 2) + ".png");
 
 yNum++;
 }
 xNum++;
 }*/

//img.updatePixels();

//image(img, 0, 0);



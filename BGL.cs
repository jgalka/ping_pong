using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;

namespace _2dGameLanguage
{
    public partial class BGL : Form
    {
        bool left;
        bool up;

        //Instance Variables
        #region
        double lastTime, thisTime, diff;
        Sprite[] sprites = new Sprite[1000];
        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        int spriteCount = 0, soundCount = 0;
        string inkey;
        int mouseKey, mouseXp, mouseYp;
        Rectangle Collision;
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        #endregion

        //Structs
        #region 
        public struct Sprite
        {
            public string image;
            public Bitmap bmp;
            public int x, y, width, height;
            public bool show;

            public Sprite(string images, int p1, int p2)
            {
                bmp = new Bitmap(images);
                image = images;
                x = p1;
                y = p2;
                width = bmp.Width;
                height = bmp.Height;
                show = true;
            }

            public Sprite(string images, int p1, int p2, int w, int h)
            {
                bmp = new Bitmap(images);
                image = images;
                x = p1;
                y = p2;
                width = w;
                height = h;
                show = true;
            }
        }

        #endregion

        public BGL()
        {
            InitializeComponent();
        }

        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;

            left = true;
            up = true;

            //Load resources and level here
            setTitle("Ping!!!");


            loadSprite("ball.bmp", 1, 320-12, 240-12);
            loadSprite("paddle.bmp", 2, 20, 240 - (136 / 2));
            loadSprite("paddle.bmp", 3, 585, 240 - (136 / 2));
            setImageColorKey(1, Color.Black);
            loadSound(1, "Photon gun shot.wav");
            setBackgroundImage("background.bmp");
        }

        private void Update(object sender, EventArgs e)
        {
            if (isKeyPressed(Keys.Up) && spriteY(2)>0)
            {
                moveSprite(2, spriteX(2), spriteY(2) - 5);
            }
            if (isKeyPressed(Keys.Down) && spriteY(2) < (480-170))
            {
                moveSprite(2, spriteX(2), spriteY(2) + 5);
            }


            //ball physics
            if (left) moveSprite(1, spriteX(1) - 5, spriteY(1));
            if (up) moveSprite(1, spriteX(1), spriteY(1) - 5);
            if (left == false) moveSprite(1, spriteX(1) + 5, spriteY(1));
            if (up == false) moveSprite(1, spriteX(1), spriteY(1) + 5);


            //border detection
            if (up && spriteY(1) < 1) up = false;
            if (up == false && spriteY(1) > (480 - 60)) up = true;

            //paddle1 detection
            if (left && spriteCollision(1, 2))
            {
                playSound(1);
                left = false;
            }

            //paddle2 detection
            if (left == false && spriteCollision(1, 3))
            {
                playSound(1);
                left = true;
            }

            // paddle2 AI
            if (spriteY(1) < spriteY(3) && spriteY(3) > 1 && left == false)
            {
                moveSprite(3, spriteX(3), spriteY(3) - 5);
            }
            if (spriteY(1) > spriteY(3) && spriteY(3) < (480 - 155) && left == false)
            {
                moveSprite(3, spriteX(3), spriteY(3) + 5);
            }

            //player win lose
            if (spriteX(1) < 1)
            {
                loadSprite("p2win.bmp", 4, 230, 190);
                setImageColorKey(4, Color.White);
            }
            if (spriteX(1) > 640)
            {
                loadSprite("p1win.bmp", 4, 230, 190);
                setImageColorKey(4, Color.White);

            }




                this.Refresh();
        }

        // Start of Game Methods

        #region

        //This is the beginning of the setter methods

        private void startTimer(object sender, EventArgs e)
        {
            timer1.Start();
            timer2.Start();
            Init();
        }

        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }


        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }
              
        }

        public void setTitle(string title)
        {
            this.Text = title;
        }

        public void setBackgroundColour(int r, int g, int b)
        {
            this.BackColor = Color.FromArgb(r, g, b);
        }

        public void setBackgroundColour(Color col)
        {
            this.BackColor = col;
        }

        public void setBackgroundImage(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        public void setBackgroundImageLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }
        
        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        public void loadSprite(string file, int spriteNum)
        {
            spriteCount++;
            sprites[spriteNum] = new Sprite(file, 0, 0);
        }

        public void loadSprite(string file, int spriteNum, int x, int y)
        {
            spriteCount++;
            sprites[spriteNum] = new Sprite(file, x, y);
        }

        public void loadSprite(string file, int spriteNum, int x, int y, int w, int h)
        {
            spriteCount++;
            sprites[spriteNum] = new Sprite(file, x, y, w, h);
        }

        public void rotateSprite(int spriteNum, int angle)
        {
            if (angle == 90)
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.Rotate90FlipNone);
            if (angle == 180)
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.Rotate180FlipNone);
            if (angle == 270)
                sprites[spriteNum].bmp.RotateFlip(RotateFlipType.Rotate270FlipNone);
        }

        public void scaleSprite(int spriteNum, int scale)
        {
            float sx = float.Parse(sprites[spriteNum].width.ToString());
            float sy = float.Parse(sprites[spriteNum].height.ToString());
            float nsx = ((sx / 100) * scale); 
            float nsy = ((sy / 100) * scale);

            sprites[spriteNum].width = Convert.ToInt32(nsx);
            sprites[spriteNum].height = Convert.ToInt32(nsy);
        }

        public void moveSprite(int spriteNum, int x, int y)
        {
            sprites[spriteNum].x = x;
            sprites[spriteNum].y = y;
        }

        public void setImageColorKey(int spriteNum, int r, int g, int b)
        {
            sprites[spriteNum].bmp.MakeTransparent(Color.FromArgb(r, g, b));
        }

        public void setImageColorKey(int spriteNum, Color col)
        {
            sprites[spriteNum].bmp.MakeTransparent(col);
        }

        public void setSpriteVisible(int spriteNum, bool ans)
        {
            sprites[spriteCount].show = ans;
        }

        public void hideSprite(int spriteNum)
        {
            sprites[spriteCount].show = false;
        }


        public void flipSprite(int spriteNum, string fliptype)
        {
            if(fliptype.ToLower() == "none")
            sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone);

            if (fliptype.ToLower() == "horizontal")
            sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);

            if (fliptype.ToLower() == "horizontalvertical")
            sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipXY);

            if (fliptype.ToLower() == "vertical")
            sprites[spriteNum].bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
        }

        public void changeSpriteImage(int spriteNum, string file)
        {
            sprites[spriteNum] = new Sprite(file, sprites[spriteNum].x, sprites[spriteNum].y);
        }

        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        public void hideMouse()
        {
            Cursor.Hide();
        }

        public void showMouse()
        {
            Cursor.Show();
        }



        //This is the beginning of the getter methods

        public bool spriteExist(int spriteNum)
        {
            if (sprites[spriteNum].bmp != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int spriteX(int spriteNum)
        {
            return sprites[spriteNum].x;
        }

        public int spriteY(int spriteNum)
        {
            return sprites[spriteNum].y;
        }

        public int spriteWidth(int spriteNum)
        {
            return sprites[spriteNum].width;
        }

        public int spriteHeight(int spriteNum)
        {
            return sprites[spriteNum].height;
        }

        public bool spriteVisible(int spriteNum)
        {
            return sprites[spriteNum].show;
        }

        public string spriteImage(int spriteNum)
        {
            return sprites[spriteNum].bmp.ToString();
        }

        public bool isKeyPressed(string key)
        {
            if (inkey == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool isKeyPressed(Keys key)
        {
            if (inkey == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool spriteCollision(int spriteNum1, int spriteNum2)
        {
            Rectangle sp1 = new Rectangle(sprites[spriteNum1].x, sprites[spriteNum1].y, sprites[spriteNum1].width, sprites[spriteNum1].height);
            Rectangle sp2 = new Rectangle(sprites[spriteNum2].x, sprites[spriteNum2].y, sprites[spriteNum2].width, sprites[spriteNum2].height);
            Collision = Rectangle.Intersect(sp1, sp2);

            if (!Collision.IsEmpty)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

        public bool isMousePressed() {
            if (mouseKey == 1) return true;
            else return false;
        }

        public int mouseX()
        {
            return mouseXp;
        }

        public int mouseY()
        {
            return mouseYp;
        }

        #endregion


        //Game Update and Input
        #region
        private void Draw(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Sprite sprite in sprites)
            {
                if (sprite.bmp != null && sprite.show == true)
                    g.DrawImage(sprite.bmp, new Rectangle(sprite.x, sprite.y, sprite.width, sprite.height));
            }
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            inkey = e.KeyCode.ToString();
        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            inkey = "";
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {
            mouseKey = 1;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
            mouseKey = 1;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {
            mouseKey = 0;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseXp = e.X;
            mouseYp = e.Y;
        }

#endregion

    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Game  
{
    class Chess
    {
        bool turn = true; //White:true , Black:false

        public Chess()
        {
            Board board = new Board();


            
            Application.Run(board);
        }
    }

    class Board:Form
    {
        private List<Piece> coloredPieces = new List<Piece>();

        public Board()
        {
            //Resize Board
            Width = 710;
            Height = 735;
            Name = "Chess";

            /*//Calculate button size related to board  size
            Size buttonSize = new Size();
            buttonSize.Width = (Width - 10) / 8;
            buttonSize.Height = (Height - 35) / 8;*/

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    //Location
                    Position pt = new Position();
                    pt.X = j;
                    pt.Y = i;

                    //////////Button set//////////
                    Piece btn;

                    //**Set Pieces**//
                    //Pawn
                    if (i == 1)
                        btn = new Pawn(pt, false);
                    else if (i == 6)
                        btn = new Pawn(pt, true);
                    //Rook
                    else if (j == 0 && i == 0 || j == 7 && i == 0)
                        btn = new Rook(pt, false);
                    else if (j == 0 && i == 7 || j == 7 && i == 7)
                        btn = new Rook(pt, true);
                    //Knight
                    else if (j == 1 && i == 0 || j == 6 && i == 0)
                        btn = new Knight(pt, false);
                    else if (j == 1 && i == 7 || j == 6 && i == 7)
                        btn = new Knight(pt, true);
                    //Bishop
                    else if (j == 2 && i == 0 || j == 5 && i == 0)
                        btn = new Bishop(pt, false);
                    else if (j == 2 && i == 7 || j == 5 && i == 7)
                        btn = new Bishop(pt, true);
                    //King
                    else if (j == 4 && i == 0)
                        btn = new King(pt, false);
                    else if (j == 4 && i == 7)
                        btn = new King(pt, true);
                    //Queen
                    else if (j == 3 && i == 0)
                        btn = new Queen(pt, false);
                    else if (j == 3 && i == 7)
                        btn = new Queen(pt, true);
                    //Else
                    else
                        btn = new Piece(pt);


                    //Add button to board
                    Controls.Add(btn);
                }
            }
        }

        //**Board(Size)

        public string GetButtonName(Position pos)
        {
            string name = "null";
            if (pos.X < 8 && pos.Y < 8 && pos.X >= 0 && pos.Y >= 0)
                name = "Button" + pos.X + pos.Y;

            return name;
        }
        public string GetButtonName(int x, int y)
        {
            string name = "null";
            if (x < 8 && y < 8 && x >= 0 && y >= 0)
                name = "Button" + x + y;

            return name;
        }
        public string GetButtonName(Position pos, int xTrans, int yTrans)
        {
            string name = "null";
            int Y = pos.Y + yTrans;
            int X = pos.X + xTrans;
            if (X < 8 && X >= 0 && Y < 8 && Y >= 0)
                name = "Button" + X + Y;

            //Console.WriteLine("X:" + x + "Y:" + y + "  Xtrans:" + xTrans + "  Ytrans:" + yTrans + "   Name:" + name + "\n");
            return name;
        }

        public void Highlight(List<Piece> pieces)
        {
            RefreshColors(coloredPieces);
            pieces[0].BackColor = Color.Blue;

            for (int i = 1; i < pieces.Count; i++)
            {
                if (pieces[i].pieceCode != -1)
                    pieces[i].BackColor = Color.Red;
                else
                    pieces[i].BackColor = Color.Green;
            }
            coloredPieces = pieces;
        }
        private void RefreshColors(List<Piece> pieces)
        {
            for (int i = 0; i < pieces.Count; i++)
            {
                if ((pieces[i].location.X + pieces[i].location.Y) % 2 == 0)
                    pieces[i].BackColor = Color.FromArgb(241, 217, 181);
                else
                    pieces[i].BackColor = Color.FromArgb(181, 136, 99);
            }
        }
    }
    class Position
    {
        public int X = -1;
        public int Y = -1;
    }

    class Piece : Button
    {
        public Position location = new Position();
        public bool isWhite;
        public int pieceCode = -1;
        

        public Piece(Position pos)
        {
            //Position in Board
            location = pos;

            //Name
            Name = "Button" + location.X + location.Y;

            //Size
            Width = 87;
            Height = 87;

            //Location in Form
            Point pt = new Point();
            pt.X = location.X * Width;
            pt.Y = location.Y * Height;
            Location = pt;

            //Style
            FlatAppearance.BorderSize = 0;
            FlatStyle = 0;

            //Background image
            BackgroundImageLayout = ImageLayout.Stretch;

            //Color
            if ((location.X + location.Y) % 2 == 0)
                BackColor = Color.FromArgb(241, 217, 181);
            else
                BackColor = Color.FromArgb(181, 136, 99);

            //Click event
            Click += new EventHandler(ClickEvent);
        }
        

        private void ClickEvent(Object sender, EventArgs e)
        { 

        }

        
    }

    class King : Piece
    {
        public King(Position pos, bool IsWhite):base(pos)
        {
            pieceCode = 0;
            isWhite = IsWhite;

            //Background image
            if (IsWhite)
                BackgroundImage = Properties.Resources.king_W;
            else
                BackgroundImage = Properties.Resources.king_B;

            //Click event
            Click += new EventHandler(ClickEvent);
        }

        private void ClickEvent(Object sender, EventArgs e)
        {
            ((Board)Parent).Highlight(((King)sender).PossibleMoves());
        }
        public List<Piece> PossibleMoves()
        {
            List<Piece> possibleMoves = new List<Piece>();
            possibleMoves.Add((Piece)this);

            for (int i = -1; i <= 1; i++)  //King move 
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (!(i == j && i == 0)) //remove piece position from possibles
                        try
                        {
                            Piece temp = (Piece)Parent.Controls.Find(((Board)Parent).GetButtonName(location, i, j), true)[0];
                            if (temp.isWhite != this.isWhite || temp.pieceCode == -1)
                                possibleMoves.Add(temp);
                        }
                        catch { }
                }
            }

            return possibleMoves;
        }

    }

    class Queen : Piece
    {
        public Queen(Position pos,bool IsWhite) : base(pos)
        {
            pieceCode = 1;
            isWhite = IsWhite;

            //Background image
            if (IsWhite)
                this.BackgroundImage = Properties.Resources.queen_W;
            else
                this.BackgroundImage = Properties.Resources.queen_B;

            this.Click += new EventHandler(ClickEvent);
        }

        private void ClickEvent(Object sender, EventArgs e)
        {
            ((Board)Parent).Highlight(((Queen)sender).PossibleMoves());
        }

        public List<Piece> PossibleMoves()
        {
            List<Piece> possibleMoves = new List<Piece>();
            possibleMoves.Add((Piece)this);

            Piece rookTemp = new Rook(location,isWhite);
            rookTemp.Name = ((Board)Parent).GetButtonName(location);
            Piece bishopTemp = new Bishop(location,isWhite);
            bishopTemp.Name = ((Board)Parent).GetButtonName(location);

            rookTemp.Parent = this.Parent;
            bishopTemp.Parent = this.Parent;

            possibleMoves.AddRange(((Rook)rookTemp).PossibleMoves())  ;
            possibleMoves.AddRange(((Bishop)bishopTemp).PossibleMoves());
            return possibleMoves;
        }

    }

    class Bishop : Piece
    {
        public Bishop(Position pos, bool IsWhite) : base(pos)
        {
            pieceCode = 2;
            isWhite = IsWhite;

            //Background image
            if (IsWhite)
                BackgroundImage = Properties.Resources.bishop_W;
            else
                BackgroundImage = Properties.Resources.bishop_B;

            //Click event
            Click += new EventHandler(ClickEvent);
        }

        private void ClickEvent(Object sender, EventArgs e)
        {
            ((Board)Parent).Highlight(((Bishop)sender).PossibleMoves());
        }


        public List<Piece> PossibleMoves()
        {
            List<Piece> possibleMoves = new List<Piece>();
            possibleMoves.Add((Piece)this);

            //Checking the mobility of the four directions
            for (int i = 1; i < 7; i++) //Down Right
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, i, i);
                    if (name != null)
                    {
                        Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);

                        if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                        {
                            possibleMoves.Add(temp1);
                            if (temp1.isWhite != this.isWhite && temp1.pieceCode != -1)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }

            for (int i = 1; i < 7; i++) //Up Left
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, -i, -i);
                    if (name != null)
                    {
                        Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);

                        if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                        {
                            possibleMoves.Add(temp1);
                            if (temp1.isWhite != this.isWhite && temp1.pieceCode != -1)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }

            for (int i = 1; i < 7; i++) //Up Right
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, -i, i);
                    if (name != null)
                    {
                        Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);

                        if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                        {
                            possibleMoves.Add(temp1);
                            if (temp1.isWhite != this.isWhite && temp1.pieceCode != -1)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }

            for (int i = 1; i < 7; i++) //Down Left
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, i, -i);
                    if (name != null)
                    {
                        Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);

                        if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                        {
                            possibleMoves.Add(temp1);
                            if (temp1.isWhite != this.isWhite && temp1.pieceCode != -1)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }
            return possibleMoves;
        }
    }

    class Knight : Piece
    {
        public Knight(Position pos, bool IsWhite):base(pos)
        {
            pieceCode = 3;
            isWhite = IsWhite;

            //Background image
            if (IsWhite)
                BackgroundImage = Properties.Resources.knight_W;
            else
                BackgroundImage = Properties.Resources.knight_B;

            //Click event
            Click += new EventHandler(ClickEvent);
        }

        private void ClickEvent(Object sender, EventArgs e)
        {
            ((Board)Parent).Highlight(((Knight)sender).PossibleMoves());
        }

        public List<Piece> PossibleMoves()
        {
            List<Piece> possibleMoves = new List<Piece>();
            possibleMoves.Add((Piece)this);

            try
            {
                string name = ((Board)Parent).GetButtonName(location, 2, 1);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }
            try
            {
                string name = ((Board)Parent).GetButtonName(location, 1, 2);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }
            try
            {
                string name = ((Board)Parent).GetButtonName(location, -2, 1);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }
            try
            {
                string name = ((Board)Parent).GetButtonName(location, 1, -2);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }
            try
            {
                string name = ((Board)Parent).GetButtonName(location, 2, -1);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }
            try
            {
                string name = ((Board)Parent).GetButtonName(location, -1, 2);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }
            try
            {
                string name = ((Board)Parent).GetButtonName(location, -2, -1);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }
            try
            {
                string name = ((Board)Parent).GetButtonName(location, -1, -2);

                Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);
                if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                    possibleMoves.Add(temp1);
            }
            catch { }

            return possibleMoves;
        }
    }

    class Rook : Piece
    {
        public Rook(Position pos, bool IsWhite):base(pos)
        {
            pieceCode = 4;
            isWhite = IsWhite;

            //Background image
            if (IsWhite)
                BackgroundImage = Properties.Resources.rook_W;
            else
                BackgroundImage = Properties.Resources.rook_B;

            //Click event
            Click += new EventHandler(ClickEvent);
        }

        private void ClickEvent(Object sender, EventArgs e)
        {
            ((Board)Parent).Highlight(((Rook)sender).PossibleMoves());
        }

        public List<Piece> PossibleMoves()
        {
            List<Piece> possibleMoves = new List<Piece>();
            possibleMoves.Add((Piece)this);

            //Checking the mobility of the four directions
            for (int i = 1; i < 7; i++)  //Up
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, i, 0);
                    if (name != null)
                    {
                        Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);

                        if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                        {
                            possibleMoves.Add(temp1);
                            if (temp1.isWhite != this.isWhite && temp1.pieceCode != -1)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }
            for (int i = -1; i > -7; i--)  //Down
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, i, 0);
                    if (name != null)
                    {
                        Piece temp1 = ((Piece)Parent.Controls.Find(name, true)[0]);

                        if (temp1.isWhite != this.isWhite || temp1.pieceCode == -1)
                        {
                            possibleMoves.Add(temp1);
                            if (temp1.isWhite != this.isWhite && temp1.pieceCode != -1)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }
            for (int i = 1; i < 7; i++)  //Right
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, 0, i);
                    if (name != null)
                    {
                        Piece temp2 = ((Piece)Parent.Controls.Find(name, true)[0]);
                        if (temp2.isWhite != this.isWhite || temp2.pieceCode == -1)
                        {
                            possibleMoves.Add(temp2);
                            if (temp2.isWhite != this.isWhite && temp2.pieceCode != -1)
                                break;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }
            for (int i = -1; i > -7; i--)  //Left
            {
                try
                {
                    string name = ((Board)Parent).GetButtonName(location, 0, i);
                    if (name != null)
                    {
                        Piece temp2 = ((Piece)Parent.Controls.Find(name, true)[0]);
                        if (temp2.isWhite != this.isWhite || temp2.pieceCode == -1)
                        {
                            possibleMoves.Add(temp2);
                            if (temp2.isWhite != this.isWhite && temp2.pieceCode != -1)
                                break;
                        }

                        else
                            break;
                    }
                    else
                        break;
                }
                catch { }
            }
            return possibleMoves;
        }

    }

    class Pawn : Piece
    {
        bool firstMove = false;
        public Pawn(Position pos, bool IsWhite):base(pos)
        {
            pieceCode = 5;
            isWhite = IsWhite;

            //Background image
            if (IsWhite)
                BackgroundImage = Properties.Resources.pawn_W;
            else
                BackgroundImage = Properties.Resources.pawn_B;

            //Click event
            Click += new EventHandler(ClickEvent);
        }

        private void ClickEvent(Object sender, EventArgs e)
        {
            ((Board)Parent).Highlight(((Pawn)sender).PossibleMoves());
        }

        public List<Piece> PossibleMoves()
        {
            List<Piece> possibleMoves = new List<Piece>();
            possibleMoves.Add((Piece)this);

            int trans = 0;

            if (isWhite)
                if (firstMove)
                    trans = -1;
                else
                    trans = -2;
            else
            {
                if (firstMove)
                    trans = 1;
                else
                    trans = 2;
            }

            try
            {
                for (int i = 1; i <= trans; i++)
                {
                    Piece temp = ((Piece)Parent.Controls.Find(((Board)Parent).GetButtonName(location, i, 0), true)[0]);

                    if (temp.pieceCode == -1)
                    {
                        possibleMoves.Add(temp);
                        if (temp.isWhite != this.isWhite && temp.pieceCode != -1)
                            break;
                    }
                    else
                        break;
                }
            }
            catch { }

            try
            {
                for (int i = -1; i >= trans; i--)
                {
                    Piece temp = ((Piece)Parent.Controls.Find(((Board)Parent).GetButtonName(location, i, 0), true)[0]);

                    if (temp.isWhite != this.isWhite || temp.pieceCode == -1)
                    {
                        possibleMoves.Add(temp);
                        if (temp.isWhite != this.isWhite && temp.pieceCode != -1)
                            break;
                    }
                }
            }
            catch { }

            try
            {
                if (isWhite)
                {
                    Piece temp = ((Piece)Parent.Controls.Find(((Board)Parent).GetButtonName(location, -1, -1), true)[0]);
                    if (temp.isWhite != isWhite && temp.pieceCode != -1)
                        possibleMoves.Add(temp);
                }
                else
                {
                    Piece temp = ((Piece)Parent.Controls.Find(((Board)Parent).GetButtonName(location, 1, 1), true)[0]);
                    if (temp.isWhite != isWhite && temp.pieceCode != -1)
                        possibleMoves.Add(temp);
                }
            }
            catch { }

            try
            {
                if (isWhite)
                {
                    Piece temp = ((Piece)Parent.Controls.Find(((Board)Parent).GetButtonName(location, -1, 1), true)[0]);
                    if (temp.isWhite != isWhite && temp.pieceCode != -1)
                        possibleMoves.Add(temp);
                }
                else
                {
                    Piece temp = ((Piece)Parent.Controls.Find(((Board)Parent).GetButtonName(location, 1, -1), true)[0]);
                    if (temp.isWhite != isWhite && temp.pieceCode != -1)
                        possibleMoves.Add(temp);
                }
            }
            catch { }

            return possibleMoves;
        }

    }
}

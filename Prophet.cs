using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpDX;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore;

namespace Prophet
{


    public class Prophet : BaseSettingsPlugin<ProphetSettings>
    {
        private List<string> PropheciesListGood;
        private List<string> PropheciesListTrash;
        private Vector2 _windowOffset;
        private IngameState _ingameState;

        public override bool Initialise()
        {
            ReadProphecies();

            _ingameState = GameController.Game.IngameState;
            _windowOffset = GameController.Window.GetWindowRectangle().TopLeft;

            return true;
        }

        public override void Render()
        {
            if (!_ingameState.IngameUi.OpenLeftPanel.IsVisible)
                return;

            GotProphecies();
        }


        private void GotProphecies()
        {
            //
            var ProphecyPanel = _ingameState.IngameUi.OpenLeftPanel.GetChildAtIndex(2)?.GetChildAtIndex(0)?.GetChildAtIndex(1)?.GetChildAtIndex(1)?.GetChildAtIndex(40)?.GetChildAtIndex(0)?.GetChildAtIndex(0);

            if ((ProphecyPanel == null) || (!ProphecyPanel.IsVisible)) 
                return;

            if (ProphecyPanel.ChildCount <= 0) 
                return;

            var foundprophsGood = new List<RectangleF>();
            var foundprophsTrash = new List<RectangleF>();

            foreach (Element element in ProphecyPanel.Children)
            {
                if (element == null)
                    continue;

                var textelement = element.GetChildAtIndex(0).GetChildAtIndex(1);
                var text = element.GetChildAtIndex(0).GetChildAtIndex(1).Text;

                if (PropheciesListGood.Contains(text))
                {
                    var drawRect = textelement.GetClientRect();
                    drawRect.Left -= 5;
                    drawRect.Right += 5;
                    drawRect.Top -= 5;
                    drawRect.Bottom += 5;
                    drawRect.X -= 5;
                    drawRect.Y -= 5;
                    foundprophsGood.Add(drawRect);
                }

                if (PropheciesListTrash.Contains(text))
                {
                    var drawRect = textelement.GetClientRect();
                    drawRect.Left -= 5;
                    drawRect.Right += 5;
                    drawRect.Top -= 5;
                    drawRect.Bottom += 5;
                    drawRect.X -= 5;
                    drawRect.Y -= 5;
                    foundprophsTrash.Add(drawRect);
                }
            }

            if (foundprophsGood.Count > 0)
            {
                DrawFrame(foundprophsGood, Settings.ColorGood);
            }

            if (foundprophsTrash.Count > 0)
            {
                DrawFrame(foundprophsTrash, Settings.ColorTrash);
            }

        }

        private void DrawFrame(List<RectangleF> Prophs, Color color)
        {
            foreach (var position in Prophs)
            {
                RectangleF border = new RectangleF { X = position.X + 8, Y = position.Y + 8, Width = position.Width - 6, Height = position.Height - 6 };
                Graphics.DrawFrame(border, color, 3);
            }
        }

        private void ReadProphecies()
        {
            PropheciesListGood = new List<string>();
            PropheciesListTrash = new List<string>();

            string pathPropGood = Path.Combine( DirectoryFullName ,"prophecies_good.txt");
            string pathPropTrash = Path.Combine( DirectoryFullName , "prophecies_bad.txt");

            CheckConfig(pathPropGood);

            using (StreamReader reader = new StreamReader(pathPropGood))
            {
                string text = reader.ReadToEnd();

                PropheciesListGood = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                reader.Close();
            }

            CheckConfig(pathPropTrash);

            using (StreamReader reader = new StreamReader(pathPropTrash))
            {
                string text = reader.ReadToEnd();

                PropheciesListTrash = text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                reader.Close();
            }
        }

        private void CheckConfig(string path)
        {
            if (File.Exists(path)) return;

            string text = "";

            using (StreamWriter streamWriter = new StreamWriter(path, true))
            {
                streamWriter.Write(text);
                streamWriter.Close();
            }
        }

    }
}

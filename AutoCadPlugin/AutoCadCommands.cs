using System.Drawing;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

using AutoCadPlugin.View;

namespace AutoCadPlugin
{
    public class AutoCadCommands : IExtensionApplication
    {
        private static PaletteSet _ps = null;

        // Функция инициализации (выполняется при загрузке плагина).
        public void Initialize()
        {

        }

        // Функция, выполняемая при выгрузке плагина.
        public void Terminate()
        {

        }

        // Функция будет вызываться при выполнении в AutoCAD команды «TestCommand».
        [CommandMethod("FiguresEditor")]
        public void FiguresEditor()
        {
            _ps = null;

            _ps = new PaletteSet("Figures Editor");
            _ps.Size = new Size(400, 600);
            _ps.DockEnabled = (DockSides)((int)DockSides.Left + (int)DockSides.Right);

            PluginWindow window = new PluginWindow();
            _ps.AddVisual("TestPlugin", window);

            _ps.KeepFocus = true;
            _ps.Visible = true;
        }
    }
}

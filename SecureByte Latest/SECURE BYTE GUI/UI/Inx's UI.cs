using Guna.UI2.WinForms;
using System.Drawing;
using System.Windows.Forms;

namespace Inx.Ui
{
    public class InxUI
    {
        public static void changeButtons(Guna2Button sender, Control container)
        {
            sender.ForeColor = Color.FromArgb(240, 80, 80);
            foreach (Control control in container.Controls)
            {
                if (control is Guna2Button guna2Button)
                {
                    guna2Button.ForeColor = (control.Name != sender.Name) ? Color.FromArgb(171, 171, 171) : guna2Button.ForeColor;
                    switch (control.Name)
                    {
                        case "gotoMain":
                            guna2Button.Image = control.Name == sender.Name ? SECURE_BYTE_GUI.Properties.Resources.main_Se : SECURE_BYTE_GUI.Properties.Resources.main_Un;
                            break;
                        case "gotoProtections":
                            guna2Button.Image = control.Name == sender.Name ? SECURE_BYTE_GUI.Properties.Resources.protections_Se : SECURE_BYTE_GUI.Properties.Resources.protections_Un;
                            break;
                        case "gotoCodeEnc":
                            guna2Button.Image = control.Name == sender.Name ? SECURE_BYTE_GUI.Properties.Resources.codeencryption_Se : SECURE_BYTE_GUI.Properties.Resources.codeencryption_Un;
                            break;
                        case "gotoJIT":
                            guna2Button.Image = control.Name == sender.Name ? SECURE_BYTE_GUI.Properties.Resources.jit_Se : SECURE_BYTE_GUI.Properties.Resources.jit_Un;
                            break;
                        case "gotoDynamic":
                            guna2Button.Image = control.Name == sender.Name ? SECURE_BYTE_GUI.Properties.Resources.dynamic_Se : SECURE_BYTE_GUI.Properties.Resources.dynamic_Un;
                            break;
                        case "gotoRenamer":
                            guna2Button.Image = control.Name == sender.Name ? SECURE_BYTE_GUI.Properties.Resources.renamer_Se : SECURE_BYTE_GUI.Properties.Resources.renamer_Un;
                            break;
                    }
                }
            }
        }
    }
}

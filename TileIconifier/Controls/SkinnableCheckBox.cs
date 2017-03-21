﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TileIconifier.Forms;
using System.Windows.Forms.VisualStyles;
using TileIconifier.Utilities;
using System.ComponentModel;

namespace TileIconifier.Controls
{
    class SkinnableCheckBox : CheckBox, ISkinnableCheckableButton
    {
        private bool basePainting;

        public override string Text
        {
            get
            {
                //Lies to the base class by telling it there is no text to draw
                //so that we can draw it ourselves.
                if (basePainting && !Enabled)
                    return "";
                else
                    return base.Text;
            }
            set { base.Text = value; }
        }

        private Color disabledForeColor = SystemColors.GrayText;
        /// <summary>
        /// Gets or sets the foreground color of the button when it is disabled.
        /// </summary>
        [DefaultValue(typeof(Color), nameof(SystemColors.GrayText))]
        public Color DisabledForeColor
        {
            get { return disabledForeColor; }
            set
            {
                if (disabledForeColor != value)
                {
                    disabledForeColor = value;
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            basePainting = true;
            base.OnPaint(pevent);
            basePainting = false;

            //We paint the disabled text on top of the base class drawing using 
            //the ForeColorDisabled color that we have implemented ourselves.
            //Incomplete implementation: We don't consider the TextImageRelation property (yet).

            if (!Enabled)
            {
                TextFormatFlags flags;
                Rectangle textRect;

                //Create flags
                flags = ButtonUtils.BaseTextFormatFlags | ButtonUtils.ConvertToTextFormatFlags(RtlTranslateContent(TextAlign));
                if (RightToLeft == RightToLeft.Yes)
                {
                    flags |= TextFormatFlags.RightToLeft;
                }

                //Create rectangle
                if (Appearance == Appearance.Button)
                {
                    textRect = ButtonUtils.GetPushButtonTextRectangle(this);
                }
                else
                {
                    textRect = ButtonUtils.GetCheckBoxTextRectangle(this, pevent.Graphics);
                }

                //Draw
                TextRenderer.DrawText(pevent.Graphics, Text, Font, textRect, DisabledForeColor, flags);
            }                
        }
    }
}

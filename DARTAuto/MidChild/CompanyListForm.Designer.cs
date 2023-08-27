namespace DARTAuto
{
    partial class CompanyListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CompanyListControl = new DevExpress.XtraGrid.GridControl();
            this.CompanyListViewControl = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.CompanyListControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanyListViewControl)).BeginInit();
            this.SuspendLayout();
            // 
            // CompanyListControl
            // 
            this.CompanyListControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CompanyListControl.Location = new System.Drawing.Point(0, 0);
            this.CompanyListControl.MainView = this.CompanyListViewControl;
            this.CompanyListControl.Name = "CompanyListControl";
            this.CompanyListControl.Size = new System.Drawing.Size(626, 935);
            this.CompanyListControl.TabIndex = 0;
            this.CompanyListControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.CompanyListViewControl});
            // 
            // CompanyListViewControl
            // 
            this.CompanyListViewControl.GridControl = this.CompanyListControl;
            this.CompanyListViewControl.Name = "CompanyListViewControl";
            // 
            // CompanyListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(626, 935);
            this.Controls.Add(this.CompanyListControl);
            this.Name = "CompanyListForm";
            this.Text = "CompanyListForm";
            ((System.ComponentModel.ISupportInitialize)(this.CompanyListControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CompanyListViewControl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl CompanyListControl;
        private DevExpress.XtraGrid.Views.Grid.GridView CompanyListViewControl;
    }
}
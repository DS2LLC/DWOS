using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DWOS.Plugin
{
    /// <summary>
    /// Interface to define a custom report to be ran from within the DWOS.Client.
    /// </summary>
    public interface IPluginCommand
    {
        void Execute(PluginContext context);
        
        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        /// <value>The name.</value>
        string Name { get; }
        
        /// <summary>
        /// Gets the description of the command.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; }
        
        /// <summary>
        /// Gets the security role identifier of the command. This will be used to identify which security group the user must be in. This can be an existing role, a new role, or empty.
        /// If the role is empty then anyone can execute the command.
        /// </summary>
        /// <value>The security role identifier.</value>
        string SecurityRoleID { get; }

        /// <summary>
        /// Gets the image to display on the toolbar.
        /// </summary>
        /// <value>The image.</value>
        System.Drawing.Image Image { get; }
    }

    /// <summary>
    /// Initialization Information for a custom report. This provides some background context for the report to use if required.
    /// </summary>
    public class PluginContext
    {
        /// <summary>
        /// Gets or sets the user identifier who is running the report.
        /// </summary>
        /// <value>The user identifier.</value>
        public int UserId { get; set; }
        /// <summary>
        /// Gets or sets the name of the user running the report.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the database connection string currently in use by DWOS.
        /// </summary>
        /// <value>The database connection string.</value>
        public string DatabaseConnectionString { get; set; }
        /// <summary>
        /// Gets or sets the company logo file path.
        /// </summary>
        /// <value>The company logo file path.</value>
        public string CompanyLogoFilePath { get; set; }
    }
}

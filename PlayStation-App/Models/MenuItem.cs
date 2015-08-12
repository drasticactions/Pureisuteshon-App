using PlayStation_App.Common;

namespace PlayStation_App.Models
{
    public class MenuItem
    {
        public string Icon { get; set; }

        public string Name { get; set; }

        public AlwaysExecutableCommand Command { get; set; }
    }
}

using System.Collections.Generic;

namespace DiscordController {
    public class Config {
        public DiscordConfig Discord { get; set; }
        public ControllerConfig Controller { get; set; }
        
        public class DiscordConfig {
            public string DiscordToken { get; set; }
            public ulong AdminUserId { get; set; }
            public ulong ListenChannelId { get; set; }
        }

        public class ControllerConfig {
            public double MinInputDuration { get; set; }
            public double MaxInputDuration { get; set; }
            public int MaxInputQueueSize { get; set; }
        
            public string DefaultProfile { get; set; }
            public Dictionary<string, Profile> Profiles { get; set; }
        }
        
        public class Profile {
            public Dictionary<string, string> Buttons { get; set; }
            public Dictionary<string, string> Sliders { get; set; }
            public Dictionary<string, Xbox360Stick> Sticks { get; set; }
        
            public class Xbox360Stick {
                public string Axis { get; set; }
                public short Direction { get; set; }

                public Xbox360Stick(string axis, short direction) {
                    this.Axis = axis;
                    this.Direction = direction;
                }
            }
        }
    }
}
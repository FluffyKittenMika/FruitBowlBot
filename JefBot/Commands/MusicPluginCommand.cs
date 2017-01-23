using Discord;
using System;
using System.Linq;
using System.Collections.Generic;
using TwitchLib;
using TwitchLib.Models.Client;
using Discord.Audio;
using NAudio.Wave;

namespace JefBot.Commands
{
    internal class MusicPluginCommand : IPluginCommand
    {
        public string PluginName => "Music";
        public string Command => "music";
        public string Help => "!music {summon}/{play} [url]";
        public IEnumerable<string> Aliases => new[] { "musicplay" };
        public bool Loaded { get; set; } = true;
        IAudioClient _vClient;

        public MusicPluginCommand()
        {
           
        }

        public void Execute(ChatCommand command, TwitchClient client)
        {
            
        }

        public async void Discord(MessageEventArgs arg, DiscordClient client)
        {

            var args = arg.Message.Text.Split(' ').ToList().Skip(1).ToList();

            try
            {
                foreach (var item in args)
                {
                    Console.WriteLine(item);
                }
                if (args.Count > 0)
                {

                    if (args[0] == "disconnect")
                    {

                        //var voicechannel = client.FindServers("Jef Chat Program").FirstOrDefault().VoiceChannels.FirstOrDefault();
                        //_vClient = await client.GetService<AudioService>().Join(voicechannel);
                        //_vClient = await client.GetService<AudioService>().Join(arg.User.VoiceChannel);
                        try
                        {

                            await _vClient.Disconnect();
                        }
                        catch (Exception e)
                        {
                            await arg.User.SendMessage(e.Message);
                            await arg.User.SendMessage("Might just not be connected to anything");
                        }
                    }
                    if (args[0] == "play")
                    {
                        int index = Bot.servers.FindIndex(f => f.Id == arg.User.Server.Id);
                        
                        Channel voicechannel = null;
                        foreach (var item in Bot.servers[index].VoiceChannels)
                        {
                            try
                            {
                                if (item.Id == arg.User.VoiceChannel.Id)
                                {
                                    voicechannel = item;
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                                await arg.User.SendMessage("You might have to rejoin the voice channel, the bot is kinda not 10/10 at this");
                            }
    
                        }

                        // non trycatch code here..
                        //  var voicechannel = (Channel)Bot.servers[index].VoiceChannels.Where(f => f.Id == arg.User.VoiceChannel.Id);

                        _vClient = await client.GetService<AudioService>() // We use GetService to find the AudioService that we installed earlier. In previous versions, this was equivelent to _client.Audio()
                        .Join(voicechannel);
                        var channelCount = arg.Server.Client.GetService<AudioService>().Config.Channels; //sound channel counts
                        var OutFormat = new WaveFormat(48000, 16, channelCount);
                        using (var MP3Reader = new Mp3FileReader(@"./This Side Out.mp3")) // Create a new Disposable MP3FileReader, to read audio from the filePath parameter
                        using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat)) // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
                        {
                            resampler.ResamplerQuality = 60; // Set the quality of the resampler to 60, the highest quality
                            int blockSize = OutFormat.AverageBytesPerSecond / 50; // Establish the size of our AudioBuffer
                            byte[] buffer = new byte[blockSize];
                            int byteCount;

                            while ((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) // Read audio into our buffer, and keep a loop open while data is present
                            {
                                if (byteCount < blockSize)
                                {
                                    // Incomplete Frame
                                    for (int i = byteCount; i < blockSize; i++)
                                        buffer[i] = 0;
                                }
                                _vClient.Send(buffer, 0, blockSize); // Send the buffer to Discord
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Data);
                Console.WriteLine(e.StackTrace);
            }
            
        }
    }
}

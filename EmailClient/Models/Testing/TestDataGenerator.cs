using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailClient.Models.Testing
{
    /// <summary>
    /// Contains methods for generating test data for the email client app.
    /// </summary>
    internal static class TestDataGenerator
    {
        /// <summary>
        /// A standard seed to ensure the randomly generated test data is consistent between runs.
        /// </summary>
        private static readonly int randSeed = 0;

        /// <summary>
        /// Lorem ipsum text to use for test content.
        /// </summary>
        private static readonly String fillerText = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, " +
            "sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Sed turpis tincidunt id " +
            "aliquet risus feugiat in ante metus. Sem nulla pharetra diam sit amet. Amet facilisis magna " +
            "etiam tempor orci. Nullam non nisi est sit amet facilisis magna etiam tempor. Viverra aliquet " +
            "eget sit amet tellus cras. Est pellentesque elit ullamcorper dignissim cras tincidunt. Leo in " +
            "vitae turpis massa sed elementum. Amet venenatis urna cursus eget. Consequat mauris nunc congue " +
            " nisi vitae suscipit tellus. Posuere morbi leo urna molestie at. Ornare massa eget egestas purus " +
            "viverra accumsan. Eros in cursus turpis massa. Ut faucibus pulvinar elementum integer." +
            "\n\n" +
            "Volutpat odio facilisis mauris sit amet. Enim ut sem viverra aliquet eget sit amet tellus. Cras " +
            "sed felis eget velit aliquet. In hac habitasse platea dictumst quisque sagittis purus sit. " +
            "Tristique senectus et netus et.Suspendisse sed nisi lacus sed viverra. Morbi tristique senectus " +
            "et netus et malesuada fames ac turpis. In metus vulputate eu scelerisque felis. Egestas maecenas " +
            "pharetra convallis posuere morbi. Rutrum quisque non tellus orci ac auctor. Nunc lobortis mattis " +
            "aliquam faucibus purus in massa tempor. Interdum posuere lorem ipsum dolor sit amet.Aliquet eget " +
            "sit amet tellus.Elit at imperdiet dui accumsan sit amet nulla facilisi morbi." +
            "\n\n" +
            "Non odio euismod lacinia at quis risus sed. Leo a diam sollicitudin tempor id eu.Vitae justo " +
            "eget magna fermentum iaculis eu non. Cursus metus aliquam eleifend mi in nulla posuere. Id diam " +
            "maecenas ultricies mi eget mauris pharetra et. Ultricies tristique nulla aliquet enim tortor. " +
            "Augue ut lectus arcu bibendum at varius vel pharetra.Dictum varius duis at consectetur.Id diam " +
            "maecenas ultricies mi. Viverra adipiscing at in tellus integer feugiat scelerisque varius.Turpis " +
            "in eu mi bibendum neque egestas congue quisque egestas. Pellentesque habitant morbi tristique " +
            "senectus et netus et malesuada fames. Pellentesque diam volutpat commodo sed egestas. Viverra " +
            "nam libero justo laoreet sit. Vitae elementum curabitur vitae nunc sed velit dignissim sodales " +
            "ut. Sollicitudin ac orci phasellus egestas tellus rutrum tellus pellentesque.Tempus iaculis urna " +
            "id volutpat lacus laoreet non curabitur gravida. Non consectetur a erat nam at. Varius vel " +
            "pharetra vel turpis." +
            "\n\n" +
            "Id diam vel quam elementum pulvinar etiam non quam lacus. Egestas erat imperdiet sed euismod " +
            "nisi porta lorem. Tortor at risus viverra adipiscing. Montes nascetur ridiculus mus mauris vitae " +
            "ultricies leo integer malesuada. Id diam maecenas ultricies mi eget mauris pharetra et.Sed enim " +
            "ut sem viverra.Blandit volutpat maecenas volutpat blandit aliquam etiam erat velit scelerisque. " +
            "Pharetra magna ac placerat vestibulum lectus mauris ultrices eros. Eget lorem dolor sed viverra. " +
            "Ullamcorper malesuada proin libero nunc consequat interdum varius." +
            "\n\n" +
            "Proin fermentum leo vel orci porta non pulvinar. Gravida arcu ac tortor dignissim convallis. " +
            "Vel elit scelerisque mauris pellentesque.Porttitor lacus luctus accumsan tortor posuere ac. " +
            "Egestas quis ipsum suspendisse ultrices gravida dictum fusce ut. Lobortis scelerisque fermentum " +
            "dui faucibus in. Vel pretium lectus quam id leo in vitae.In est ante in nibh mauris cursus " +
            "mattis molestie a. Penatibus et magnis dis parturient montes nascetur. Nullam non nisi est sit " +
            "amet facilisis magna etiam.Id interdum velit laoreet id donec. Sit amet purus gravida quis " +
            "blandit. Lobortis feugiat vivamus at augue eget arcu dictum. Aliquam nulla facilisi cras " +
            "fermentum odio eu feugiat pretium. Eget magna fermentum iaculis eu non diam phasellus vestibulum " +
            "lorem. Semper quis lectus nulla at volutpat diam ut venenatis.";

        /// <summary>
        /// Generates a set of messages with incremental headers and filled with random content. The
        /// messages are randomly generated but are consistent between calls.
        /// </summary>
        /// <param name="count">The number of messages to generate. Must be at least zero.</param>
        /// <returns>A list of randomly generated messages.</returns>
        internal static List<Message> GetTestMessages(int count)
        {
            // Validate at least one message will be generated.
            if (count < 0)
            {
                /// <exception cref="ArgumentOutOfRangeException">Count must be greater than or equal to zero.</exception>
                throw new ArgumentOutOfRangeException("count", count, "Count must be greater than or equal to zero.");
            }

            // Initialize the random number generator with a constant seed. This ensures messages,
            //      while randomly generated, are consistent between calls.
            Random rand = new Random(TestDataGenerator.randSeed);

            // Create a new list of messages.
            List<Message> messages = new List<Message>();

            // Fill the list of massages with the given number of messages.
            for (int i = 0; i < count; i++)
            {
                // Randomly generate a start point and length of the filler text to use. The -1 ensures at
                //      least one character is included.
                int textStart = rand.Next(fillerText.Length - 1);
                int textlength = rand.Next(fillerText.Length - textStart);

                // Initialize a new message and add it to the list.
                messages.Add(new Message()
                {
                    Id = Guid.NewGuid(),
                    Subject = "Message #" + (i + 1),
                    Body = TestDataGenerator.fillerText.Substring(textStart, textlength)
                });
            }

            // Return the list of newly generated messages.
            return messages;
        }
    }
}

module Agents

open MyImage

/// <summary>
/// Represents a logger, which receives messages and prints them
/// to the console
/// </summary>
let logger =

    MailboxProcessor.Start(fun inbox ->
        async {
            while true do
                let! message = inbox.Receive()
                printfn $"{message}"
        })

/// <summary>
/// Represents a message type that can either contain an image or
/// an end-of-stream signal, which is used with an asynchronous reply
/// channel
/// </summary>
type imageMessage =
    | Image of MyImage
    | EOS of AsyncReplyChannel<unit>

/// <summary>
/// Defines an image saver agent that listens to image-messages and
/// saves them to a specified output directory
/// </summary>
/// <param name="outputDirectory">The directory where the images will be saved</param>
let imageSaver outputDirectory =

    let initial (inbox: MailboxProcessor<imageMessage>) =

        async {
            while true do
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    logger.Post "Image saver is finished! "
                    channel.Reply()

                | Image image ->
                    save image (Helper.generatePath outputDirectory image.Name)
                    logger.Post $"Saved: %A{image.Name}! "
        }

    MailboxProcessor.Start initial

/// <summary>
/// Defines an image processing agent that listens to image-messages and
/// applies the given function
/// </summary>
/// <param name="imageEditor">The image editing function to apply to incoming images</param>
/// <param name="receiver">The MailboxProcessor that receives the processed images</param>
let imageProcessor imageEditor (receiver: MailboxProcessor<_>) =

    let initial (inbox: MailboxProcessor<imageMessage>) =

        async {
            while true do
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    logger.Post "Image processor is finished! "
                    receiver.PostAndReply EOS
                    channel.Reply()
                | Image image ->
                    let filtered = imageEditor image
                    logger.Post $"Edited: %A{image.Name}! "
                    receiver.Post(Image filtered)
        }

    MailboxProcessor.Start initial

/// <summary>
/// Represents a message type encapsulated either a string path
/// or an end-of-stream message, which is used with an asynchronous
/// reply channel
/// </summary>
type pathMessage =
    | Path of string
    | EOS of AsyncReplyChannel<unit>

/// <summary>
/// Creates an image processing and saving MailboxProcessor that continuously receives path messages from
/// an input mailbox. It loads, edits, and saves images to the specified output directory
/// </summary>
/// <param name="imageEditor">The image editing function to apply to the loaded images</param>
/// <param name="outputDirectory">The directory where the edited images will be saved</param>
let imageFullProcessor imageEditor outputDirectory =

    let initial (inbox: MailboxProcessor<pathMessage>) =

        async {
            while true do
                let! message = inbox.Receive()

                match message with
                | EOS channel ->
                    logger.Post "Image processor and saver is finished! "
                    channel.Reply()

                | Path path ->
                    let image = load path
                    logger.Post $"Opened: %A{image.Name}! "
                    let filtered = imageEditor image
                    logger.Post $"Edited: %A{image.Name}! "
                    save filtered (Helper.generatePath outputDirectory image.Name)
                    logger.Post $"Saved: %A{image.Name}! "

        }

    MailboxProcessor.Start initial

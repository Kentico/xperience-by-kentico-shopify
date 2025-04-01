namespace Kentico.Xperience.Shopify.Config
{
    /// <summary>
    /// Class for Shopify synchronization configuration.
    /// </summary>
    public class SynchronizationConfig
    {
        /// <summary>
        /// Workspace name for synchronized content items.
        /// </summary>
        public required string WorkspaceName { get; set; }

        /// <summary>
        /// Target folder of synchronization of products
        /// </summary>
        public required SynchronizationFolder ProductFolder { get; set; }

        /// <summary>
        /// Target folder of synchronization of product variants
        /// </summary>
        public required SynchronizationFolder ProductVariantFolder { get; set; }

        /// <summary>
        /// Target folder of synchronization of images
        /// </summary>
        public required SynchronizationFolder ImageFolder { get; set; }
    }

    /// <summary>
    /// Synchronization folder info.
    /// </summary>
    public class SynchronizationFolder
    {
        /// <summary>
        /// Folder ID.
        /// </summary>
        public required int FolderID { get; set; }

        /// <summary>
        /// Folder GUID.
        /// </summary>
        public required Guid FolderGuid { get; set; }

        /// <summary>
        /// Folder display code name.
        /// </summary>
        public required string FolderCodeName { get; set; }
    }
}

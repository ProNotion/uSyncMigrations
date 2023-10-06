using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Cms.Core.PropertyEditors;
using uSync.Migrations.Migrators.Models;
using uSync.Migrations.Migrators.Optional;
using uSync.Migrations.Models;

namespace uSync.Migrations.Tests.Migrators;

[TestFixture]
public class NestedToBlockListMigratorTests : MigratorTestBase
{
    [SetUp]
    public override void Setup()
    {
        base.Setup();
        var logger = new Mock<ILogger<NestedToBlockListMigrator>>();
        _migrator = new NestedToBlockListMigrator(logger.Object);
    }
    
    /// <inheritdoc />
    protected override SyncMigrationDataTypeProperty GetMigrationDataTypeProperty() =>
        new SyncMigrationDataTypeProperty("Test", UmbConstants.PropertyEditors.Aliases.NestedContent,
            "Nvarchar",
            new List<PreValue>
            {
                new PreValue
                {
                    SortOrder = 1, Alias = "contentTypes",
                    Value =
                        "[\n  {\n    \"ncAlias\": \"myContentTypeAlias\",\n    \"ncTabAlias\": \"Content\",\n    \"nameTemplate\": \"{{title}}\"\n  }\n]"
                },
                new PreValue {SortOrder = 2, Alias = "minItems", Value = "0"},
                new PreValue {SortOrder = 3, Alias = "maxItems", Value = "0"},
                new PreValue {SortOrder = 4, Alias = "confirmDeletes", Value = "1"},
                new PreValue {SortOrder = 5, Alias = "showIcons", Value = "1"},
                new PreValue {SortOrder = 6, Alias = "hideLabel", Value = ""}
            });
            
    private SyncMigrationDataTypeProperty GetMigrationDataTypePropertyWithMultipleContentTypes() =>
        new SyncMigrationDataTypeProperty("NestedContent", UmbConstants.PropertyEditors.Aliases.NestedContent,
            "Nvarchar",
            new List<PreValue>
            {
                new PreValue
                {
                    SortOrder = 1, Alias = "contentTypes",
                    Value =
                        "[{\"ncAlias\":\"contentAutoList\",\"ncTabAlias\":\"Content\",\"nameTemplate\":\"{{$index}}. {{title}}\"},{\"ncAlias\":\"ContentCustomItem\",\"ncTabAlias\":\"Content\",\"nameTemplate\":\"{{$index}}. {{title}}\"},{\"ncAlias\":\"contentManualList\",\"ncTabAlias\":\"Content\",\"nameTemplate\":\"{{$index}}. {{title}}\"}]"
                },
                new PreValue {SortOrder = 2, Alias = "minItems", Value = "0"},
                new PreValue {SortOrder = 3, Alias = "maxItems", Value = "0"},
                new PreValue {SortOrder = 4, Alias = "confirmDeletes", Value = "1"},
                new PreValue {SortOrder = 5, Alias = "showIcons", Value = "1"},
                new PreValue {SortOrder = 6, Alias = "hideLabel", Value = ""}
            });

    protected override SyncMigrationContentProperty GetMigrationContentProperty(string value) => 
        new SyncMigrationContentProperty("Test", "NestedContent", UmbConstants.PropertyEditors.Aliases.NestedContent,
            value);

    /// <inheritdoc />
    [Test]
    public override void DatabaseTypeAsExpected() => DatabaseTypeAsExpectedBase("Nvarchar");

    /// <inheritdoc />
    [Test]
    public override void EditorAliasAsExpected() =>
        EditorAliasAsExpectedBase(UmbConstants.PropertyEditors.Aliases.BlockList);

    /// <inheritdoc />
    [Test]
    public override void ConfigValueAsExpected()
    {
        var nestedContentConfig = _migrator!.GetConfigValues(GetMigrationDataTypeProperty(), _context!);
        
        // Deserialize the config to a BlockListConfiguration object so we can use the same method to convert it to a
        // JSON string for comparison. Other we end up with failed tests due to different ordering of the properties
        // even though the output is essentially the same.
        var obj = JsonConvert.DeserializeObject<BlockListConfiguration>("{\r\n  \"Blocks\": [\r\n    {\r\n      \"contentElementTypeKey\": \"00000000-0000-0000-0000-000000000000\",\r\n      \"forceHideContentEditorInOverlay\": false,\r\n      \"label\": \"{{title}}\"\r\n    }\r\n  ],\r\n  \"UseInlineEditingAsDefault\": false,\r\n  \"UseLiveEditing\": false,\r\n  \"UseSingleBlockMode\": false,\r\n  \"ValidationLimit\": {}\r\n}");
        var expected = ConvertResultToJsonTestResult(obj);

        Assert.AreEqual(expected, ConvertResultToJsonTestResult(nestedContentConfig));
    }

    public override void ContentValueAsExpected(string value, string expected) =>
        ContentValueAsExpectedBase(value, expected);

    [Test]
    public void PropertyValue_Is_As_Expected()
    {
        var value = "[{\"key\":\"fbd0c8a1-9ea8-43a4-86e6-3f461673ed69\",\"name\":\"United Kingdom\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"United Kingdom\\\",\\\"udi\\\":\\\"umb://document/89bae9cc4f06470bad4444673f4f6292\\\"}]\",\"icon\":73383},{\"key\":\"dc93dc7d-8159-4111-ae9d-f5fb5a6488c7\",\"name\":\"Česká republika\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"Česká republika\\\",\\\"udi\\\":\\\"umb://document/de3f36ae43d94760b7a14ac47da90791\\\"}]\",\"icon\":73380},{\"key\":\"9beff9b5-e4f0-4f43-84e7-1114edc65b21\",\"name\":\"China\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"China\\\",\\\"udi\\\":\\\"umb://document/b3623ab207214147910e2478e8ad92f6\\\"}]\",\"icon\":73400},{\"key\":\"a0143886-66ad-4f17-9768-cdbdf456b3d0\",\"name\":\"Deutschland\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"Deutschland\\\",\\\"udi\\\":\\\"umb://document/6cb8779b12254475ba0f10ad307fd082\\\"}]\",\"icon\":73381},{\"key\":\"3de55d63-ef65-4f62-8ff0-7c269a05c389\",\"name\":\"España\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"España\\\",\\\"udi\\\":\\\"umb://document/0a4858c9dd9d43229c5ffb65fbcd78dd\\\"}]\",\"icon\":73382},{\"key\":\"a03c38c5-6e62-426c-837d-d4284f9c4b63\",\"name\":\"EU (Brussels)\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"EU (Brussels)\\\",\\\"udi\\\":\\\"umb://document/9c17f49167ba439b8c4f2850fec7d300\\\"}]\",\"icon\":73401},{\"key\":\"aeb639bc-10ef-412a-9221-b83f7201cfac\",\"name\":\"France\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"France\\\",\\\"udi\\\":\\\"umb://document/5a204578399c4c2f99b1815b7f5fa4e4\\\"}]\",\"icon\":73389},{\"key\":\"b44f8a9b-6911-4d48-b2ee-25aacc26c484\",\"name\":\"International (ciwf.org)\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"International (ciwf.org)\\\",\\\"udi\\\":\\\"umb://document/2904d64b977643fa87d67359a60ac8d5\\\"}]\",\"icon\":102545},{\"key\":\"990a8ce2-e506-4a19-86d1-79588688d602\",\"name\":\"Italia\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"Italia\\\",\\\"udi\\\":\\\"umb://document/4d8dedf373814b30b0704ac171114cc3\\\"}]\",\"icon\":73384},{\"key\":\"1e1f45d5-87d2-4793-bb65-dcf872f2f252\",\"name\":\"Nederland\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"Nederland\\\",\\\"udi\\\":\\\"umb://document/79749fdb7e994c429592e0a5c80ec7bc\\\"}]\",\"icon\":73385},{\"key\":\"e8c1d06f-1899-4b0f-9364-cb09f93577fb\",\"name\":\"Polska\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"Polska\\\",\\\"udi\\\":\\\"umb://document/c4471b42bf7942c79d460761f4b060f6\\\"}]\",\"icon\":73386},{\"key\":\"891b5536-8d28-408a-94fe-3b96abaa1d4a\",\"name\":\"Sverige\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"Sverige\\\",\\\"udi\\\":\\\"umb://document/8a909bd40c714c469ce164af75d76df9\\\"}]\",\"icon\":73387},{\"key\":\"f1a4f056-dd29-4fd0-8d37-46402793bf03\",\"name\":\"United States\",\"ncContentTypeAlias\":\"siteMenuItem\",\"link\":\"[{\\\"name\\\":\\\"United States\\\",\\\"udi\\\":\\\"umb://document/9d45b5c2f34e4c02a34725cf8dc9fce9\\\"}]\",\"icon\":73388}]";
        var convertedValue = _migrator.GetContentValue(GetMigrationContentProperty(value), _context!);
        
        Assert.IsNotNull(convertedValue);
    }

    [Test]
    public void ConfigValue_As_Expected_For_NestedContent_With_Multiple_ContentTypes()
    {
        var nestedContentConfig =
            _migrator!.GetConfigValues(GetMigrationDataTypePropertyWithMultipleContentTypes(), _context!);
        
        // Deserialize the config to a BlockListConfiguration object so we can use the same method to convert it to a
        // JSON string for comparison. Other we end up with failed tests due to different ordering of the properties
        // even though the output is essentially the same.
        var obj = JsonConvert.DeserializeObject<BlockListConfiguration>(
            "{\"Blocks\":[{\"contentElementTypeKey\":\"00000000-0000-0000-0000-000000000000\",\"label\":\"{{$index}}. {{title}}\",\"forceHideContentEditorInOverlay\":false},{\"contentElementTypeKey\":\"00000000-0000-0000-0000-000000000000\",\"label\":\"{{$index}}. {{title}}\",\"forceHideContentEditorInOverlay\":false},{\"contentElementTypeKey\":\"00000000-0000-0000-0000-000000000000\",\"label\":\"{{$index}}. {{title}}\",\"forceHideContentEditorInOverlay\":false}],\"validationLimit\":{},\"useSingleBlockMode\":false,\"useLiveEditing\":false,\"useInlineEditingAsDefault\":false}");
        var expected = ConvertResultToJsonTestResult(obj);

        Assert.AreEqual(expected, ConvertResultToJsonTestResult(nestedContentConfig));

    }
}
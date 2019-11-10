# ConfigGenerator
用于 Unity 的配置填写工具

适用于小型项目，可以方便地把特定格式的 Excel 表格直接转译为运行时的 C# 实例。
其功能主要分为两步骤：
 * 编辑器下把 Excel 转译为 xml
 * 运行时把 xml 转译为 C# Object

以后会争取实现 json 等其他格式

# 快速开始
 1. 把 Packages/com.flyingsnow.configgenerator 拷贝到你的项目 Packages 里
 2. 建立 Assets/ConfigSheets 存放 Excel 文件
 3. 建立 Assets/StreamingAssets/Config 存放 xml 文件
 4. 按下面的格式要求编写你的 Excel 文件，点击菜单 **Config/Translate Config** 命令翻译成 xml
 5. 按照格式要求定义 C# class 并绑定
 6. 编写代码并运行

# Excel 表格的说明

Excel **文件的名字不重要**，你可以凭自己的喜好把工作表分配到任何一个文件中。工具会自动解析每一个 .xlsx/.xls 文件的每一张工作表，工作表的名字**重要**，请遵守规范。

## 工作表

### 工作表的类型

工作表分三种：Setting、Template 和 Relation，有效的工作表命名必须以这三个单词之一结尾。以 ~ 结尾的表不导出，这样的表名字和内容可以任意。

1. Setting

Setting 是一种全局的配置，表格中只有一行数据，代码中会把这一行数据导出成一个 object。比如工作表 NPCSetting
	
timeFactor|npcCount|assetPath
float|int|string
时空缩放|同屏最大 NPC 数量|资源路径
3.1415|10|http://download.game.com

导出的 xml：
```xml
<?xml version="1.0" encoding="UTF-8"?>
<data name="NPCSetting" type="setting">
	<timeFactor type="float">3.1415</timeFactor>
	<npcCount type="int">10</npcCount>
	<assetPath type="string">http://download.game.com</assetPath>
</data>
```

对应的 C# class:

```C#
public class NPCSetting
{
    public float timeFactor { get; private set; }
    public int npcCount { get; private set; }
    public string assetPath { get; private set; }
}
```

2. Template

Template 是多行的数据，其中第一列必须为 id，id 是一个整数值，作为 这一行数据的 key，比如工作表 NPCTemplate:

id|name|area|posX|posY
id|string|Area|int|int
ID|名字|所属区域|地图位置X|地图位置Y
10000|王小二|CHINA|-213|47
10001|李大狗|CHINA|-182|-32
10002|Tanaka|JAPAN|-182|109
10003|赵七炫|KOREA|-4|128

导出 xml：

```xml
<?xml version="1.0" encoding="utf-8"?>
<data type="template" name="NPCTemplate">
  <NPCTemplate id="100">
    <name type="string">王小二</name>
    <nationality type="Nationality">CHINA</nationality>
  </NPCTemplate>
  <NPCTemplate id="101">
    <name type="string">李大狗</name>
    <nationality type="Nationality">CHINA</nationality>
  </NPCTemplate>
  <NPCTemplate id="102">
    <name type="string">Tanaka</name>
    <nationality type="Nationality">JAPAN</nationality>
  </NPCTemplate>
  <NPCTemplate id="103">
    <name type="string">赵七炫</name>
    <nationality type="Nationality">KOREA</nationality>
  </NPCTemplate>
</data>
```

对应的 C# class:

```C#

```

其中 Nationality 是自定义的枚举类型

3. Relation

Relation 也是多行的数据，表示两个 Template 之间的关系，其中第一列和第二列必须为 id，作为这一行数据的 key，比如工作表 NPCCityRelation:

npcID|cityID|hour
npc_id|city_id|int[]
NPC ID|城市 ID|出现时间
1000|2000|
1000|2001|3;4;5
1000|2002|1;2;10

导出 xml:

 ```xml
 <?xml version="1.0" encoding="utf-8"?>
<data type="relation" name="NPCCityRelation">
  <NPCCityRelation npc_id="1000" city_id="2000">
    <hour type="[]" itemType="int" />
  </NPCCityRelation>
  <NPCCityRelation npc_id="1000" city_id="2001">
    <hour type="[]" itemType="int">
      <int type="int">3</int>
      <int type="int">4</int>
      <int type="int">5</int>
    </hour>
  </NPCCityRelation>
  <NPCCityRelation npc_id="1000" city_id="2002">
    <hour type="[]" itemType="int">
      <int type="int">1</int>
      <int type="int">2</int>
      <int type="int">10</int>
    </hour>
  </NPCCityRelation>
</data>
 ```

对应的 C# class:
```C#
```

### 工作表的格式

工作表每一列的前三行分别是：变量名、类型、说明，前两行任一行空白的列不会导出，可以填写任意内容（比如行注释）。

变量名必须是合法的 C# 变量名。

类型目前支持：

 * 基本类型 int、float、string
 * 特殊类型 id
 * 自定义枚举类型
 * 以上各类型的数组类型如 int[]、float[]、string[]

说明可以任意填写。


# C# class 的说明





# SearchControls

This control supports WinForm.

The control of fuzzy searching by table.

利用表格进行模糊查找的控件。

## Release Notes

### 1.2.4

ButtonFlowLayoutPanelMethod中重写BtnFoundClick的Handle处理方式，让BtnFoundClick事件专注于重写后台抓取，不用重写UI，UI会自动刷新。  
修正Bug

### 1.2.3

ButtonFlowLayoutPanelMethod添加FoundCompleted事件  
修复bug

### 1.2.2

SearchTextBox添加Enter事件:  
SearchTextBox.Enter在搜索表格出现之前触发,  
((TextBox)SearchTextBox).Enter在搜索表格出现之后触发。  

修复bug。

### 1.2.1  

添加搜索框朝左的选项  
升级NPOI至2.5.1  
修复大小发生变化时，搜索框没有移动的问题

### 1.2.0.11  

修复bug，处理DataError事件

### 1.2.0.10  

修复移动滚动条时出现异常的Bug

### 1.2.0.9  

修复重新出现搜索框时没有出现焦点的bug

### 1.2.0.8  

修复由线程造成的异常bug

### 1.2.0.7  

给创建多音列的方法CreateManyInitialsDataColumn  
添加多线程方案CreateManyInitialsDataColumnAsync

### 1.2.0.6  

按钮群添加IsEscQuit  
按钮群方法添加 查找和更新中发生错误时的事件

### 1.2.0.5  

修正窗口不跟随文本框移动的bug

### 1.2.0.4  

修正bug

### 1.2.0.3  

更新SqlClient至1.1.1

### 1.2.0.2  

追加版本说明

### 1.2.0.1  

修正错误的注释

### 1.2  

追加:  
操控表格的按钮群(ButtonFlowLayoutPanel)、  
按钮群的按钮方法(ButtonFlowLayoutPanelMethod)、  
按钮方法的接口(IButtonFlowLayoutPanelMethod)  
状态栏(GridStatusStrip)、  
以NOPI为基础,将DataTable转换为Excel、Word(命名空间:Export)

### 1.1  

模糊查找文本框控件(SearchTextBox)、  
模糊查找表格控件(SearchDataGridView)
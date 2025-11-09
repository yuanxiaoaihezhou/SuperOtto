# 贡献指南 | Contributing Guide

感谢您对SuperOtto项目的兴趣！我们欢迎所有形式的贡献。

Thank you for your interest in the SuperOtto project! We welcome all forms of contributions.

## 🤝 如何贡献 | How to Contribute

### 报告问题 | Reporting Issues

如果您发现bug或有功能建议：

1. 检查是否已存在类似的Issue
2. 创建新Issue并使用合适的标签
3. 详细描述问题或建议
4. 如果是bug，请提供复现步骤

If you find a bug or have a feature suggestion:

1. Check if a similar issue already exists
2. Create a new issue with appropriate labels
3. Provide detailed description
4. For bugs, include reproduction steps

### 提交代码 | Submitting Code

1. **Fork仓库** | Fork the repository
2. **创建分支** | Create a branch
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **进行更改** | Make your changes
4. **测试更改** | Test your changes
   ```bash
   dotnet build
   dotnet run
   ```
5. **提交更改** | Commit your changes
   ```bash
   git commit -m "Add: Brief description of your changes"
   ```
6. **推送分支** | Push your branch
   ```bash
   git push origin feature/your-feature-name
   ```
7. **创建Pull Request** | Create a Pull Request

## 📝 代码规范 | Code Standards

### C# 代码风格 | C# Code Style

- 使用PascalCase命名公共成员
- 使用_camelCase命名私有字段
- 使用camelCase命名局部变量
- 添加XML文档注释到公共API

Example:
```csharp
/// <summary>
/// Description of the class
/// </summary>
public class MyClass
{
    private int _privateField;
    
    /// <summary>
    /// Description of the method
    /// </summary>
    public void MyMethod(int parameter)
    {
        var localVariable = parameter + _privateField;
    }
}
```

### 提交信息格式 | Commit Message Format

使用清晰的提交信息格式：

```
Type: Brief description (50 chars or less)

More detailed explanation if needed (wrap at 72 chars)

- Bullet points are okay
- Use present tense: "Add feature" not "Added feature"
```

**类型 (Type)**:
- `Add`: 新功能
- `Fix`: Bug修复
- `Update`: 更新现有功能
- `Refactor`: 代码重构
- `Docs`: 文档更新
- `Test`: 测试相关
- `Build`: 构建系统更改

## 🎨 资源贡献 | Asset Contributions

### 替换程序化生成的资源

如果您想为游戏提供美术资源：

1. 保持与程序化生成资源相同的尺寸
2. 提供多种分辨率（如需要）
3. 使用PNG格式（透明背景）
4. 在PR中说明资源的用途和许可

### 资源规格 | Asset Specifications

- **瓦片**: 32x32 像素
- **角色**: 32x32 像素（可扩展到精灵表）
- **作物**: 32x32 像素，多个生长阶段
- **UI图标**: 32x32 像素
- **格式**: PNG with alpha channel

## 🧪 测试 | Testing

在提交PR之前：

1. 确保代码编译无错误
   ```bash
   dotnet build
   ```

2. 手动测试您的更改
   ```bash
   dotnet run
   ```

3. 测试您的更改在不同场景下的表现

## 📚 文档 | Documentation

### 更新文档

如果您的更改影响到用户或开发者：

- 更新README.md（如需要）
- 更新ARCHITECTURE.md（如果更改了架构）
- 更新ROADMAP.md（如果完成了路线图项目）
- 添加代码注释解释复杂逻辑

### 文档风格

- 使用中英双语
- 保持简洁明了
- 使用代码示例
- 添加图表（如适用）

## 🌟 代码审查流程 | Code Review Process

1. **自我审查**: 提交前审查自己的代码
2. **CI检查**: 确保GitHub Actions构建通过
3. **代码审查**: 维护者会审查您的PR
4. **反馈**: 根据反馈进行必要的修改
5. **合并**: 审查通过后将合并到主分支

## 🎯 贡献建议 | Contribution Ideas

### 初学者友好 | Good First Issues

- 添加新的作物类型
- 改进程序化生成的纹理
- 添加新的工具
- 翻译文档到其他语言
- 修复typos

### 中级任务 | Intermediate Tasks

- 实现保存/加载系统
- 添加音效系统
- 优化渲染性能
- 实现NPC系统基础

### 高级任务 | Advanced Tasks

- 实现多人游戏支持
- 创建模组系统
- 重构大型系统
- 性能分析和优化

## 💬 交流 | Communication

- **GitHub Issues**: 报告bug和功能请求
- **Pull Requests**: 讨论代码更改
- **Discussions**: 一般性讨论和问题

## 📜 行为准则 | Code of Conduct

### 我们的承诺 | Our Pledge

我们致力于为每个人提供友好、安全和包容的环境。

We are committed to providing a friendly, safe, and welcoming environment for all.

### 期望行为 | Expected Behavior

- 尊重不同的观点和经验
- 接受建设性的批评
- 关注对社区最有利的事情
- 对其他社区成员表示同理心

### 不可接受的行为 | Unacceptable Behavior

- 骚扰性评论
- 人身攻击
- 发布他人私人信息
- 其他不专业或不受欢迎的行为

## 📋 清单 | Checklist

提交PR前，确保：

- [ ] 代码遵循项目风格指南
- [ ] 代码编译无错误
- [ ] 手动测试了更改
- [ ] 更新了相关文档
- [ ] 提交信息清晰明确
- [ ] 分支基于最新的主分支

## 🙏 感谢 | Acknowledgments

感谢所有贡献者让SuperOtto变得更好！

Thank you to all contributors for making SuperOtto better!

## ❓ 问题？| Questions?

如果您有任何问题，请随时：

- 在GitHub Issues中提问
- 查看现有文档
- 参考ARCHITECTURE.md了解项目结构

If you have any questions, feel free to:

- Ask in GitHub Issues
- Check existing documentation
- Refer to ARCHITECTURE.md for project structure

---

再次感谢您的贡献！🎮

Thank you again for your contribution! 🎮

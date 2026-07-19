const path = require('path');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');
const CssMinimizerPlugin = require('css-minimizer-webpack-plugin');
const TerserPlugin = require('terser-webpack-plugin');

module.exports = {
    mode: 'development',//开发模式
    //mode: 'production',//优化打包输出和构建性能的模式
    entry: {
        'site': './Script/init.ts'
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'), // 输出路径
        filename: 'js/[name].js', // 输出文件名
        clean: true, // 清除目标目录
    },
    resolve: {
        //设置类型可以作为模块被引用
        extensions: [".ts", ".tsx", ".js"],
        alias: {
            "@": path.resolve(__dirname, "."),
        },
    },
    module: {
        rules: [
            {
                test: /\.tsx?$/, // 匹配 TypeScript 文件
                use: "ts-loader", // 使用 ts-loader 处理 TypeScript 文件
                exclude: /node_modules/ // 排除 node_modules 文件夹
            },
            {
                test: /\.(css|scss)$/,
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader,
                        options: {
                            esModule: true,
                        },
                    },
                    "css-loader",//处理css文件
                    'sass-loader',//编译SCSS文件
                ],
            }
        ],
    },
    plugins: [
        new MiniCssExtractPlugin(
            {
                filename: 'css/[name].css',
            }
        ),
    ],
    optimization: {
        minimize: true, // 开启代码压缩
        minimizer: [
            `...`,
            new CssMinimizerPlugin({
                minimizerOptions: {
                    preset: [
                        'default',
                        {
                            discardComments: { removeAll: true },
                        },
                    ],
                },
            }), // 压缩css
            new TerserPlugin({
                terserOptions: {
                    format: {
                        comments: false,      // 压缩时删除所有注释
                    },
                },
                extractComments: false,   // 不提取 *.LICENSE.txt
            }), // 压缩js
        ],
    },
};
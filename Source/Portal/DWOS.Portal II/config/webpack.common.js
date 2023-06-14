var webpack = require('webpack');
var MiniCssExtractPlugin = require("mini-css-extract-plugin");
var OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');
var helpers = require('./helpers');

module.exports = {
    entry: {
        'dependencies': './src/dependencies.ts',
        'build': './src/main.ts'
    },

    devtool: 'source-map',

    output: {
        path: helpers.root('scripts'),
        filename: '[name].js',
        chunkFilename: '[id].chunk.js'
    },

    resolve: {
        extensions: ['.ts', '.js']
    },

    module: {
        rules: [
            {
                test: /\.ts$/,
                loaders: [
                    {
                        loader: 'awesome-typescript-loader',
                        options: { configFileName: helpers.root('tsconfig.json') }
                    }, 'angular2-template-loader'
                ]
            },
            {
                test: /\.html$/,
                loader: 'html-loader'
            },
            {
                test: /\.(png|jpe?g|gif|svg|woff|woff2|ttf|eot|ico)$/,
                loader: 'file-loader?name=assets/[name].[ext]'
            },
            {
                test: /\.css$/,
                exclude: helpers.root('src', 'app'),
                use: [
                    {
                        loader: MiniCssExtractPlugin.loader
                    },
                    'css-loader'
                ]
            },
            {
                test: /\.css$/,
                include: helpers.root('src', 'app'),
                loader: 'raw-loader'
            }
        ]
    },

    plugins: [
        new MiniCssExtractPlugin({
            filename: '[name].css'
        }),
        new OptimizeCssAssetsPlugin()
    ]
};

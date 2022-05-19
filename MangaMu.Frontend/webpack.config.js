const path = require('path');
const { VueLoaderPlugin } = require('vue-loader');

module.exports = {
    entry: './src/index.js',
    devServer: {
        static: {
            directory: path.resolve(__dirname, 'dist'),
        },
        compress: false,
        port: 3000,
    },
    output: {
        filename: 'main.js',
        path: path.resolve(__dirname, 'dist'),
    },
    resolve: {
        alias: {
            '~': path.resolve(__dirname, './src/'),
        },
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader',
            },
            {
              test: /\.png$/,
              use: {
                loader: "url-loader",
                options: { limit: 8192 }
              }
            },
            {
                test: /\.s[ac]ss$/i,
                use: [
                    // Creates `style` nodes from JS strings
                    'style-loader',
                    // Translates CSS into CommonJS
                    'css-loader',
                    // Compiles Sass to CSS
                    'sass-loader',
                ],
            },
            {
                test: /\.css$/,
                // the order of `use` is important!
                use: [{ loader: 'style-loader' }, { loader: 'css-loader' }],
            }
        ],
    },
    plugins: [
        new VueLoaderPlugin()
    ],
};

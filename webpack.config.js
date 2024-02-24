const path = require('path')
const HtmlWebpackPlugin = require('html-webpack-plugin')

var config = {
    entry: './wwwroot/src/js/index.js',
    module: {
        rules: [
            { test: /\.svg$/, use: 'svg-inline-loader' },
            { test: /\.css$/, use: ['style-loader', 'css-loader'] },
            { test: /\.(js)$/, use: 'babel-loader' }
        ]
    },
    output: {
        path: path.resolve(__dirname, 'wwwroot/dist'),
        filename: 'index_bundle.js'
    },
    plugins: [
        new HtmlWebpackPlugin()
    ],
};


module.exports = (env, argv) => {
    if (argv.mode === 'development') {
      config.devtool = 'source-map';
    }
  
    if (argv.mode === 'production') {
      //...
    }
  
    return config;
  };
//#region 自定义的方法

//获取某个字段值的集合(已剔重)
function GetArrByFd(fdName) {
    var fdArr = [];
    if (dataArr != null && dataArr.length > 0) {
        for (var i = 0; i < dataArr.length; i++) {
            if (isInArray(fdArr, dataArr[i][fdName]))
            { continue; }
            else
            {
                fdArr.push(dataArr[i][fdName]);
            }
        }
    }
    return fdArr;
}

//返回集合,格式：[{value:"",name:""},{value:"",name:""}...]
function GetSeries(name, value) {
    var fdArr = [];
    if (dataArr != null && dataArr.length > 0) {
        for (var i = 0; i < dataArr.length; i++) {
            var obj = { value: dataArr[i][value], name: dataArr[i][name] };
            fdArr.push(obj);
        }
    }
    return fdArr;
}

/**
使用jquery的inArray方法判断元素是否存在于数组中
@param {Object} arr 数组
@param {Object} value 元素值
*/
function isInArray(arr, value) {
    var index = $.inArray(value, arr);
    if (index >= 0) {
        return true;
    }
    return false;
}

//数组剔重
Array.prototype.unique = function () {
    var res = [];
    var json = {};
    for (var i = 0; i < this.length; i++) {
        if (!json[this[i]]) {
            res.push(this[i]);
            json[this[i]] = 1;
        }
    }
    return res;
}

//数值排序
function NumAscSort(a, b) {
    return a - b;
}
function NumDescSort(a, b) {
    return b - a;
}

//#endregion



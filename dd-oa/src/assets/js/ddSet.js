import * as dd from 'dingtalk-jsapi'
import dayjs from 'dayjs'

function infoCode(corpId){
    return new Promise(function(resolve, reject) {
        dd.ready(function() {
            dd.runtime.permission.requestAuthCode({
                corpId, // 企业id
                onSuccess: function (info) {
                    resolve(info)
                },
                onFail : function(err) {
                    setToast({text:JSON.stringify(err)})
                }
            });
        });
    })
}
//显示load
async function showLoad(text = '使劲加载中..'){
    dd.device.notification.showPreloader({
        text, //loading显示的字符，空表示不显示文字
        showIcon: true, //是否显示icon，默认true
        onSuccess : function(result) {
            /*{}*/
        },
        onFail : function(err) {}
    })
}
//隐藏load
async function hideLoad(){
    dd.device.notification.hidePreloader({
        onSuccess : function(result) {
            /*{}*/
        },
        onFail : function(err) {}
    })
}
//toast
function setToast({icon = '',text = '',duration = 3,delay = 0} = {}){
    return new Promise(function(resolve, reject) {
        dd.device.notification.toast({
            icon, //icon样式，有success和error，默认为空
            text, //提示信息
            duration, //显示持续时间，单位秒，默认按系统规范[android只有两种(<=2s >2s)]
            delay, //延迟显示，单位秒，默认0
            onSuccess : function(result) {
                setTimeout(function () {
                    resolve(true)
                },duration*1000)
                /*{}*/
            },
            onFail : function(err) {}
        })
    })
}
//设置下拉控件
function setChosen({source = [],selectedKey = ''} = {}){
    return new Promise(function(resolve, reject) {
        dd.biz.util.chosen({
            source,
            selectedKey, // 默认选中的key
            onSuccess : function(result) {
                resolve(result) 
            },
            onFail : function(err) {
                dd.device.notification.toast({
                    icon: '', //icon样式，有success和error，默认为空
                    text: err, //提示信息
                    duration: 3, //显示持续时间，单位秒，默认按系统规范[android只有两种(<=2s >2s)]
                    delay: 0, //延迟显示，单位秒，默认0
                })
            }
        })
    })
}
//日期时间
function setDatepicker({format = 'yyyy-MM-dd',value = ''} = {}){
    return new Promise(function(resolve, reject) {
        dd.biz.util.datepicker({
            format,//注意：format只支持android系统规范，即2015-03-31格式为yyyy-MM-dd
            value, //默认显示日期
            onSuccess : function(result) {
                resolve(result) 
                //onSuccess将在点击完成之后回调
                /*{
                    value: "2015-02-10"
                }
                */
            },
        })
    })
}
//日期区间
function setChooseInterval({defaultStart = 0,defaultEnd = 0} = {}){
    return new Promise(function(resolve, reject) {
        dd.biz.calendar.chooseInterval({
            defaultStart:dayjs().valueOf(),
		    defaultEnd:dayjs().add(1, 'day').valueOf(),
            onSuccess : function(result) {
                resolve(result) 
                //onSuccess将在点击完成之后回调
                /*{
                    value: "2015-02-10"
                }
                */
            },
        })
    })
}
//确认提示框
function setConfirm(message,button0Text,button1Text){
    return new Promise(function(resolve, reject) {
        dd.device.notification.confirm({
            message:message || '确定操作吗？',
            title: "提示",
            buttonLabels: [button0Text || '确定', button1Text || '取消'],
            onSuccess : function(result) {
                resolve(result) 
                //onSuccess将在点击button之后回调
                /*
                {
                    buttonIndex: 0 //被点击按钮的索引值，Number类型，从0开始
                }
                */
            },
            onFail : function(err) {}
        })
    })
}

export default {
    infoCode,
    setToast,
    setChosen,
    setDatepicker,
    setChooseInterval,
    showLoad,
    hideLoad,
    setConfirm
}
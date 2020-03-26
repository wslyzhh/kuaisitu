<!--  -->
<template>
    <div>

    </div>
</template>

<script>
import * as dd from 'dingtalk-jsapi'
export default {
    name:"",
    data() {
       return {

       };
    },
    props:{
        title:{
            type:String,
            default:''
        },
        show:{
            type:Boolean,
            default:true
        },
        control:{
            type:Boolean,
            default:true
        },
        text:{
            type:String,
            default:''
        }
    },
    components: {},
    computed: {},
    created(){
        this.setTitleRight({title:this.title,show:this.show,control:this.control,text:this.text})
    },
    mounted() {
        
    },
    methods: {
        setTitleRight({title = '',show = true,control = true,text = ''} = {}){
            let _this = this
            dd.biz.navigation.setTitle({
                title, //控制标题文本，空字符串表示显示默认文本
            });
            dd.biz.navigation.setRight({
                show,//控制按钮显示， true 显示， false 隐藏， 默认true
                control,//是否控制点击事件，true 控制，false 不控制
                text,//控制显示文本，空字符串表示显示默认文本
                onSuccess : function(result) {
                    _this.$emit('rightClick')
                },
                onFail : function(err) {
                }
            })
        }
    },
}
</script>

<style scoped lang='scss'>

</style>

<!-- 选择器 -->
<template>
    <div class="choose_all_list" v-if="show" @click="hide">
        <div class="choose_content" @click.stop="">
            <ul class="all_list">
                <li :class="{active:item.ratio}" v-for="(item,index) in list" :key="index">
                    <span>{{item.de_name}} </span>
                    <input type="text" v-model="item.ratio" :title="item.de_subname" @blur="checkRatio($event)">%
                </li>
            </ul>
            <div class="choose_btn">
                <button @click="changeReset">重置</button>
                <button class="affirm" @click="changeAffirm">确认</button>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    name:"",
    data() {
       return {
           chooselist:[]
       };
    },
    props:{
        show:{
            type:Boolean,
            default:false
        },
        list:{
            type:Array,
            default:[]
        },
		type:{
		    type:Number,
		    default:1 // 1 单选 2 多选
        }
    },
    components: {},
    computed:{
       
    },
    created(){
    },
    mounted() {
    },
    methods: {
        checkRatio(event){
            let _this = this
            let thisRatio = event.target.value
            if (thisRatio && (!(/(^[1-9]\d*$)/.test(thisRatio)) || thisRatio <= 0 || thisRatio >= 100)) {
                _this.ddSet.setToast({text:'业绩比例须为正整数，且大于0小于100'})
                event.target.value=''
                return
            }            
            else{
                _this.list.map((item,index) => {
                    if(item.de_subname == event.target.title){
                        if(thisRatio){
                            _this.$set(item,'ratio',thisRatio)
                        }else{
                            _this.$set(item,'ratio',null)
                        }
                        
                    }                    
                })
                
            }         
        },
        changeReset(){
            this.list.map((item,index) => {
                this.$set(item,'ratio',null)
            })
        },
        changeAffirm(){
            let activeList = []
            let totalRatio=0
            this.list.map((item,index) => {                
                if(item.ratio){
                    totalRatio += parseInt(item.ratio)
                    activeList.push(item)
                }
            })
            if (totalRatio >= 100 || totalRatio<0) {
                this.ddSet.setToast({text:'业绩比例须为大于0,小于等于100的正整数,且业绩比例之和必须小于100'})
            }
            else{
                this.$emit('on-affirm',activeList)
                this.hide()
            }
        },
        hide(){
            this.$emit('update:show',false);
        }
    },
}
</script>

<style scoped lang='scss'>
    .greenColor{
        color:green;
    }
    .redColor{
        color:red;
    }
    .orangeColor{
        color:orange;
    }
    .choose_all_list{
        position: fixed;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
        display: flex;
        align-items: center;
        justify-content: center;
        background-color: rgba(0,0,0,.1);
        .choose_content{
            width: 80%;
            background-color:#FFF;
            border-radius: 4px;
            overflow: hidden;
            .all_list{
                padding: 15px;
                max-height: 7rem;
                overflow-y: auto;
                li{
                    position: relative;
                    padding: 15px 0;
                    font-size: .28rem;
                    color: #333;
                    padding-right: .4rem;
                    height: 0.5rem;
                    border-bottom: 1px solid #f1f1f1;
                    span{
                        width: 1.0rem;
                        display: block;
                        float: left;
                        height: 0.5rem;
                        line-height: 0.5rem;
                        margin-right: 2rem;
                    }
                    input{
                        border: 1px solid #f1f1f1;
                        width: 1.5rem;
                        //float: right;
                        height: 0.5rem;
                    }
                }
            }
            .choose_btn{
                display: flex;
                justify-content: space-around;
                border-top: 1px solid #f1f1f1;
                button{
                    outline: none;
                    background: none;
                    border: none;
                    flex: 1;
                    height: .8rem;
                    line-height: .8rem;
                    color:#333;
                    font-size: .3rem;
                    background-color: #f7f7f7;
                    &.affirm{
                        background-color: rgb(79,148,241);
                        color: #FFF;
                    }
                }
            }
        }
        
    }
</style>

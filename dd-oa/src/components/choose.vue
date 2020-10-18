<!-- 选择器 -->
<template>
    <div class="choose_all_list" v-if="show" @click="hide">
        <div class="choose_content" @click.stop="">
            <ul class="all_list">
                <li :class="{active:item.isChecked}" @click="activeItem(item)" v-for="(item,index) in list" :key="index">{{item.name}} {{ item.orderCount}}</li>
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
        },
        showNum:{
            type:Boolean,
            default:false
        }
    },
    components: {},
    computed: {},
    mounted() {

    },
    methods: {
        activeItem(item){
            if(!item.isChecked){
				if(1 == this.type){
					this.changeReset();
				}
				this.$set(item,'isChecked',true)
            }else{
                this.$set(item,'isChecked',!item.isChecked)
            }
        },
        changeReset(){
            this.list.map((item,index) => {
                this.$set(item,'isChecked',false)
            })
        },
        changeAffirm(){
            let activeList = []
            this.list.map((item,index) => {
                if(item.isChecked){
                    activeList.push(item)
                }
            })
            this.$emit('on-affirm',activeList)
            this.hide()
        },
        hide(){
            this.$emit('update:show',false);
        }
    },
}
</script>

<style scoped lang='scss'>
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
                    border-bottom: 1px solid #f1f1f1;
                    &:after{
                        content: '';
                        position: absolute;
                        right: 0;
                        top: 50%;
                        transform: translateY(-50%);
                        width: .35rem;
                        height: .35rem;
                        border-radius: 50%;
                        border: 1px solid #CCC;
                        box-sizing: border-box;
                    }
                    &.active:after{
                        content: '';
                        background-image: url('../assets/img/icon_choose.png');
                        background-size: .34rem;
                        background-repeat: no-repeat;
                        background-position: center;  
                        border: none;                      
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

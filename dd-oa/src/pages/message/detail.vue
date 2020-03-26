<!-- 添加客户 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="li_auto flex">
                <label class="title"><span>标题</span></label>
                {{addData.me_title}}
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>内容</span></label>
                <div class="content">{{addData.me_content}}</div>
            </li>
        </ul>
        <top-nav :title='"查看消息"' :text='"删除"' @rightClick="submit" ></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
export default {
    name:"",
    data() {
        return {
            addData:{},
            me_id:0            
        };
    },
    components: {},
    computed: {},
    created(){
        let {me_id,isRead} = this.$route.query
        this.me_id=me_id
        let params = {
                me_id:me_id
            }
        if(isRead == false){
            this.readMessage(params).then(res => {
                if(res.data.msg=='success'){
                    this.ddSet.setToast({text:'阅读成功'})
                }
            })
            
        }
        this.getMessageDetails(params).then(res => {
            this.addData = res.data                
        })
    },
    mounted() {
    },
    methods: {
        ...mapActions([
            'getMessageDetails',
            'deteleMessage',
            'readMessage'
        ]),
        submit(item){ //提交
            let _this = this;
            _this.ddSet.setConfirm('确定要删除消息吗？').then(res=>{
                if(0 == res.buttonIndex){
                    _this.ddSet.showLoad()
                    _this.deteleMessage({
                            me_id:_this.me_id
                        }).then((res) => {
                            _this.ddSet.hideLoad()
                            if(res.data.status){
                                _this.ddSet.setToast({text:'删除成功'}).then(res => {
                                    _this.$router.go(-1)
                                })
                            }else{
                                _this.ddSet.setToast({text:res.data.msg})
                            }
                    }).catch(err => {
                        _this.ddSet.hideLoad()
                    })
                }
            })            
        }
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
    .flex_s_b{
        line-height: 0.4rem
    }
    .content{
        padding: 20px 0
    }
</style>


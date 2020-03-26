<!-- 添加客户 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="flex flex_a_c flex_s_b" @click="changeFlag">
                <label class="title"><span>审批状态</span></label>
                <input type="text" readonly :value="o_flagText">
			    <div class="icon_right arrows_right"></div>
            </li>
        </ul>
        <top-nav :title='"审批订单"' :text='"保存"' @rightClick="submit" ></top-nav>
    </div>
</template>

<script>
import {mapActions,mapState} from 'vuex'
export default {
    name:"",
    data() {
        return {
            addData:{
                orderID:'',
                managerid:0   // TODO:测试当前登录人ID
                },            
            type:'',
            o_flag:0,
            o_flagText:'待审批',
            flagList:[  //审批状态
                {
                    key:'待审批',
                    value:'0'
                },
                {
                    key:'审批未通过',
                    value:'1'
                },
                {
                    key:'审批成功',
                    value:'2'
                },
            ]
        };
    },
    components: {},
    computed: {	
        ...mapState({
            userInfo: state => state.user.userInfo
        })
    },
    created(){ 
        let oid= this.$route.query.id
        this.addData.orderID = oid
        this.getOneOrderData()   
    },
    mounted() {
    },
    methods: {
        ...mapActions([
            'getOrderDetails',
            'checkOrder'
        ]),
        submit(item){ //提交
            let _this = this
            if(!_this.o_flag){
                _this.ddSet.setToast({text:'请选择审批状态'})
                return
            }
            _this.addData.o_flag = _this.o_flag
            _this.addData.managerid = _this.userInfo.id //测试ID
            _this.ddSet.showLoad()
            console.log(_this.addData)
            _this.checkOrder(_this.addData).then(res => {
                _this.ddSet.hideLoad()
                if(res.data.status){
                    _this.ddSet.setToast({text:'审核成功'}).then(res => {
                        _this.$router.go(-1)
                    })
                }else{
                    _this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                _this.ddSet.hideLoad()
            })
        },
        getOneOrderData(){
            let _this = this
			_this.getOrderDetails({orderID:_this.addData.orderID}).then(res => {
				if(!res.data){
					return;
				}
                let tmpData = res.data;
				for (var key in tmpData) { 
					if('string' == typeof tmpData[key]){
						_this.$set(_this.addData,key,tmpData[key])
					}
                }
                _this.o_flagText = res.data.o_flagText
			})
		},
        changeFlag(){
            let _this = this
                let source =_this.flagList,selectedKey = _this.o_flagText          
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this,'o_flag',res.value)
                    _this.$set(_this,'o_flagText',res.key)
                })
        }
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
    .looK_button_list{
        li{
            outline: none;
            background: none;
            border: none;
            width: 6.9rem;
            height: .8rem;
            line-height: .8rem;
            text-align: center;
            color: #FFF;
            font-size: .36rem;
            margin: 0 auto;
            border-radius: 4px;
            margin-bottom: .2rem;
        }
    }
</style>


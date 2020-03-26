<!-- 编辑非业务支付申请 -->
<template>
    <div class="body_gery">
        <ul class="form_list form_list_noborder">
            <li class="flex flex_a_c" @click="chosenPayType">
                <label class="title"><span class="must">支付类别</span></label>
                <input type="text" readonly v-model="addData.uba_type_text" placeholder="请选择支付类别">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex li_auto" v-if="addData.uba_type>0 ">
                <label class="title"><span class="must">支付用途</span></label>
                <textarea v-model="addData.uba_function" placeholder="请输入支付用途内容"></textarea>
            </li>
            <li class="flex flex_a_c" @click="chosenPayFunction" v-else>
                <label class="title"><span class="must">支付用途</span></label>
                <input type="text" readonly v-model="addData.uba_function" placeholder="请选择支付用途">
                <div class="icon_right arrows_right"></div>
            </li>
            <li class="flex li_auto">
                <label class="title"><span>用途说明</span></label>
                <textarea v-model="addData.uba_instruction" placeholder="请输入用途说明内容"></textarea>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收款银行</span></label>
                <input type="text" v-model="addData.uba_receiveBank" placeholder="请输入收款银行">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>账户名称</span></label>
                <input type="text" v-model="addData.uba_receiveBankName" placeholder="请输入账户名称">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>收款账号</span></label>
                <input type="text" v-model="addData.uba_receiveBankNum" placeholder="请输入收款账号">
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span class="must">金额</span></label>
                <input type="text" v-model="addData.uba_money" placeholder="请输入有效金额">
            </li>
            <li class="flex flex_a_c flex_s_b" @click="selectDate">
                <label class="title"><span class="must">预付日期</span></label>
                <input type="text" readonly :value="addData.uba_foreDate" placeholder="请选择日期"  >
                <div class="icon_right time"></div>
            </li>
            <li class="li_auto flex">
                <label class="title"><span>备注</span></label>
                <textarea v-model="addData.uba_remark" placeholder="请输入备注"></textarea>
            </li>
            <li class="flex flex_a_c flex_s_b">
                <label class="title"><span>附件</span></label>
				<div class="icon_right accessory">
					<input type="file" multiple="multiple" ref="FileUp" @change="upFile($event)">
				</div>
            </li>
			<li class="flex flex_a_c flex_s_b" v-for="(f,index) in files" :key="index">
				<a :href="'/' + f.pp_filePath">{{f.pp_fileName}}</a>
				<span>{{f.pp_size}}K</span>
			    <div class="icon_right delete"	@click="delPFile(f.pp_id,f.pp_fileName)"></div>
			</li>
        </ul>
        <top-nav :title='type == "add" ? "添加非业务支付信息":"编辑非业务支付信息"' :text='"保存"' @rightClick="submit"></top-nav>
    </div>
</template>

<script>
import {
	mapActions,
	mapState
} from 'vuex'
import {formatDate} from '../../assets/js/date.js'

export default {
    name:"",
    data() {
        return {
            addData:{},
            then:0,
            type:'',
            paytype:0,
            payfunction:0,
            oID:'',
           files:[],
           fileData:''
        };
    },
    filters:{
        formatDate(time){
            let date = new Date(time)
            return formatDate(date,'yyyy-MM-dd')
        }
    },
    components: {},
    computed: {
        ...mapState(            
            {
            userInfo: state => state.user.userInfo
        }) 
    },
    created(){
        let {type,oID,paytype,payfunction} = this.$route.query
        this.type = type
        this.paytype = paytype
        this.payfunction = payfunction
        this.addData.uba_oid = oID
    },
    mounted() {
    },
    methods: {
        ...mapActions([
            'getUnBusinessNature',
            'getUnBusinessPayFunction',
            'getUnBusinessPayDetails',
            'getUnBusinessPayAdd',
            'getUpLoadFile',
            'delFile'
        ]),
        submit(item){ //提交
            let _this = this
            //console.log(this.addData.uba_type)
            if(!_this.addData.uba_type_text){
                _this.ddSet.setToast({text:'支付类别不能为空'})
                return
            }
            if(!_this.addData.uba_function){
                _this.ddSet.setToast({text:'支付用途不能为空'})
                return
            }
            if(!_this.addData.uba_money){
                _this.ddSet.setToast({text:'请填写金额'})
                return
            }
            if(!_this.addData.uba_foreDate){
                _this.ddSet.setToast({text:'预付日期不能为空'})
                return
            }
            //this.addData.uba_oid = this.oID
            _this.addData.managerid = _this.userInfo.id //测试ID
            _this.ddSet.showLoad()
            this.getUnBusinessPayAdd(_this.addData).then(res => {
                if(res.data.status){
                    //上传附件                                      
                    if(_this.fileData.length > 0){
                        var data = new FormData();
                        Object.keys(this.fileData).map((item,index)=>{
                            data.append("file",this.fileData[index])                
                        }) 
                        data.append("type",2)
                        data.append("keyID",res.data.uba_id)
                        data.append("fileType",2)
                        data.append("managerid",_this.addData.managerid)
                        _this.getUpLoadFile(data).then(res => {
                            _this.ddSet.hideLoad()
                            if(res.data.status){
                                _this.ddSet.setToast({text:'新增信息成功'}).then(res => {
                                    _this.$router.go(-1)
                                })
                            }
                            else{
                                _this.ddSet.setToast({text:res.data.msg})
                            }
                        })
                    }
                    else{
                        _this.ddSet.hideLoad()
                        _this.ddSet.setToast({text:'新增信息成功'}).then(res => {
                            _this.$router.go(-1)
                        })
                    }
                }else{
                    _this.ddSet.hideLoad()
                    _this.ddSet.setToast({text:res.data.msg})
                }
            }).catch(err => {
                _this.ddSet.hideLoad()
            })
        },
        selectDate(){ //活动日期
			let _this = this
			_this.ddSet.setDatepicker().then(res => {
                _this.$set(this.addData,'uba_foreDate',res.value)
            })
        },                    
        chosenPayType(){   //支付类别
            let _this = this
            this.getUnBusinessNature({type:this.paytype}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.addData.uba_type
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                	source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'uba_type',res.value)
                    _this.$set(_this.addData,'uba_type_text',res.key)
                })
            })
        },                 
        chosenPayFunction(){   //支付用途
            let _this = this
            console.log(this.payfunction)
            this.getUnBusinessPayFunction({type:this.payfunction}).then(res => {
                //console.log(res)
                let source = []
                let selectedKey = _this.addData.uba_function
                res.data.map((item,index) => {
                    let obj = {
                        key:item.value,
                        value:item.key
                    }
                    source.push(obj)
                })
                _this.ddSet.setChosen({source,selectedKey}).then(res => {
                    _this.$set(_this.addData,'uba_function',res.value)
                    _this.$set(_this.addData,'uba_function_text',res.key)
                })
            })
        },
        upFile(e){
            let _this = this
            _this.fileData = _this.$refs.FileUp.files
            Object.keys(_this.fileData).map((item,index)=>{
                _this['files'].push({
                    pp_fileName:_this.fileData[index].name,
                    pp_id:0
                })
            })     
        },
		delPFile(_fid,_fileName){
            let _this = this;
            _this.ddSet.setConfirm('确定要删除《'+_fileName+'》文件吗？').then(res=>{
                if(0 == res.buttonIndex){
                    _this.ddSet.showLoad()
                    if(_fid){
                        _this.delFile({
					   	fileID:_fid,
					   	type:2,
					   	managerid:_this.addData.managerid
                        }).then(res => {
                            _this.ddSet.hideLoad()
                            if(1 == res.data.status){
                                _this.ddSet.setToast({text:'删除文件成功'})
                                _this['files'] = _this['files'].filter(function(item){
                                    return item.pp_id != _fid
                                })
                            }
                            else{
                                _this.ddSet.setToast({text:res.data.msg})
                            }
                        }).catch(err => {
                            _this.ddSet.hideLoad()
                        })
                    }
                    else{
                        _this.ddSet.hideLoad()
                        _this['files'] = _this['files'].filter(function(item){
                            return item.pp_id != _fid
                        })
                    }
                }
            }) 			
		}
    },
    beforeDestroy(){
        
    }
}
</script>

<style scoped lang="scss">
        
</style>


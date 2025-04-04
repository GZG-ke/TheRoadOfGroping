import { defineStore } from "pinia";
import {
  type userType,
  store,
  router,
  resetRouter,
  routerArrays,
  storageLocal
} from "../utils";
import { useMultiTagsStoreHook } from "./multiTags";
import {
  type DataInfo,
  type ConfigurationInfo,
  setToken,
  userKey,
  removeToken,
  setConfig,
  setUser,
  userConfig
} from "@/utils/auth";
import {
  type UserLoginDto,
  type FeatureListDto,
  type TokenInfoOutput,
  AccountsServiceProxy,
  FeaturesServiceProxy
} from "@/shared";
const accountServiceProxy = new AccountsServiceProxy();
const featuresServiceProxy = new FeaturesServiceProxy();
export const useUserStore = defineStore({
  id: "pure-user",
  state: (): userType => ({
    // 头像
    avatar: storageLocal().getItem<DataInfo>(userKey)?.avatar ?? "",
    // 用户名
    username: storageLocal().getItem<DataInfo>(userKey)?.userName ?? "",
    // 昵称
    nickname: storageLocal().getItem<DataInfo>(userKey)?.nickName ?? "",
    // 页面级别权限
    roles: storageLocal().getItem<ConfigurationInfo>(userConfig)?.roles ?? [],
    // 按钮级别权限
    permissions:
      storageLocal().getItem<ConfigurationInfo>(userConfig)?.permissions ?? [],
    // 是否勾选了登录页的免登录
    isRemembered: false,
    // 登录页的免登录存储几天，默认7天
    loginDay: 7
  }),
  actions: {
    /** 存储头像 */
    SET_AVATAR(avatar: string) {
      this.avatar = avatar;
    },
    /** 存储用户名 */
    SET_USERNAME(userName: string) {
      this.username = userName;
    },
    /** 存储昵称 */
    SET_NICKNAME(nickName: string) {
      this.nickname = nickName;
    },
    /** 存储角色 */
    SET_ROLES(roles: Array<string>) {
      this.roles = roles;
    },
    /** 存储按钮级别权限 */
    SET_PERMS(permissions: Array<string>) {
      this.permissions = permissions;
    },
    /** 存储是否勾选了登录页的免登录 */
    SET_ISREMEMBERED(bool: boolean) {
      this.isRemembered = bool;
    },
    /** 设置登录页的免登录存储几天 */
    SET_LOGINDAY(value: number) {
      this.loginDay = Number(value);
    },
    /** 登入 */
    async loginByUsername(data) {
      return new Promise<UserLoginDto>((resolve, reject) => {
        accountServiceProxy
          .login(data)
          .then(res => {
            if (res) {
              setUser(res.userInfoOutPut);
              setToken(res.tokenInfoOutput);
              resolve(res);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },

    async getUserConfiger() {
      return new Promise<FeatureListDto>((resolve, reject) => {
        featuresServiceProxy
          .getUserConfigurations()
          .then(res => {
            if (!!res) {
              setConfig(res);
              resolve(res);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    },
    /** 前端登出（不调用接口） */
    logOut() {
      this.username = "";
      this.roles = [];
      this.permissions = [];
      removeToken();
      useMultiTagsStoreHook().handleTags("equal", [...routerArrays]);
      resetRouter();
      router.push("/login");
    },
    /** 刷新`token` */
    async handRefreshToken(data) {
      return new Promise<TokenInfoOutput>((resolve, reject) => {
        accountServiceProxy
          .refrshToken(data)
          .then(res => {
            if (res) {
              setToken(res);
              resolve(res);
            }
          })
          .catch(error => {
            reject(error);
          });
      });
    }
  }
});

export function useUserStoreHook() {
  return useUserStore(store);
}

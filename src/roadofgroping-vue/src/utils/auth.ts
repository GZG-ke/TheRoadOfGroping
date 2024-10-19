import Cookies from "js-cookie";
import { useUserStoreHook } from "@/store/modules/user";
import { storageLocal, isString, isIncludeAllChildren } from "@pureadmin/utils";
import type { FeatureListDto, TokenInfoOutput, UserInfoOutPut } from "@/shared";
import moment from "moment";
// import { ExitStatus } from "typescript";

export interface DataInfo {
  /** 头像 */
  avatar?: string;
  /** 用户名 */
  userName?: string;
  /** 昵称 */
  nickName?: string;
}
export interface TokenInfo<T> {
  /** token */
  accessToken: string;
  /** `accessToken`的过期时间（时间戳） */
  expires: T;
  /** 用于调用刷新accessToken的接口时所需的token */
  refreshToken: string;
}
export interface ConfigurationInfo {
  /** 当前登录用户的角色 */
  roles?: Array<string>;
  /** 当前登录用户的按钮级别权限 */
  permissions?: Array<string>;
}

export const userKey = "user-info";
export const userConfig = "user-config";
export const userToken = "user-token";
export const TokenKey = "Authorization";
/**
 * 通过`multiple-tabs`是否在`cookie`中，判断用户是否已经登录系统，
 * 从而支持多标签页打开已经登录的系统后无需再登录。
 * 浏览器完全关闭后`multiple-tabs`将自动从`cookie`中销毁，
 * 再次打开浏览器需要重新登录系统
 * */
export const multipleTabsKey = "multiple-tabs";

/** 获取`token` */
export function getToken(): TokenInfo<number> {
  // 此处与`TokenKey`相同，此写法解决初始化时`Cookies`中不存在`TokenKey`报错
  return Cookies.get(TokenKey)
    ? JSON.parse(Cookies.get(TokenKey))
    : storageLocal().getItem(userToken);
}

/**
 * @description 设置`token`以及一些必要信息并采用无感刷新`token`方案
 * 无感刷新：后端返回`accessToken`（访问接口使用的`token`）、`refreshToken`（用于调用刷新`accessToken`的接口时所需的`token`，`refreshToken`的过期时间（比如30天）应大于`accessToken`的过期时间（比如2小时））、`expires`（`accessToken`的过期时间）
 * 将`accessToken`、`expires`、`refreshToken`这三条信息放在key值为authorized-token的cookie里（过期自动销毁）
 * 将`avatar`、`userName`、`nickName`、`roles`、`permissions`、`refreshToken`、`expires`这七条信息放在key值为`user-info`的localStorage里（利用`multipleTabsKey`当浏览器完全关闭后自动销毁）
 */
export function setUser(data: UserInfoOutPut) {
  function setUserKey({ avatar, userName, nickName }) {
    useUserStoreHook().SET_AVATAR(avatar);
    useUserStoreHook().SET_USERNAME(userName);
    useUserStoreHook().SET_NICKNAME(nickName);
    storageLocal().setItem(userKey, {
      avatar,
      userName,
      nickName
    });
  }
  if (data.userName) {
    const { userName } = data;
    setUserKey({
      avatar: data?.avater ?? "",
      userName,
      nickName: data?.nickName ?? ""
    });
  } else {
    const avatar = storageLocal().getItem<DataInfo>(userKey)?.avatar ?? "";
    const userName = storageLocal().getItem<DataInfo>(userKey)?.userName ?? "";
    const nickName = storageLocal().getItem<DataInfo>(userKey)?.nickName ?? "";
    setUserKey({
      avatar,
      userName,
      nickName
    });
  }
}
export function setToken(data: TokenInfoOutput) {
  let expires = 0;
  const { accessToken, refreshToken } = data;
  const { isRemembered, loginDay } = useUserStoreHook();
  // 判断 data.expires 是不是 Moment 对象
  expires = moment.isMoment(data.expires)
    ? data.expires.valueOf()
    : new Date(data.expires).getTime();
  // 如果后端直接设置时间戳，将此处代码改为expires = data.expires，然后把上面的DataInfo<Date>改成DataInfo<number>即可
  const cookieString = JSON.stringify({ accessToken, expires, refreshToken });
  expires > 0
    ? Cookies.set(TokenKey, cookieString, {
        expires: (expires - Date.now()) / 86400000
      })
    : Cookies.set(TokenKey, cookieString);
  Cookies.set(
    multipleTabsKey,
    "true",
    isRemembered
      ? {
          expires: loginDay
        }
      : {}
  );
  storageLocal().setItem(userToken, {
    accessToken,
    refreshToken,
    expires
  });
}
export function setConfig(data: FeatureListDto) {
  function setUserConfig({ roles, permissions }) {
    useUserStoreHook().SET_ROLES(roles);
    useUserStoreHook().SET_PERMS(permissions);
    storageLocal().setItem(userConfig, {
      roles,
      permissions
    });
  }
  if (data.roles) {
    const { roles, permissions } = data;
    setUserConfig({
      roles,
      permissions
    });
  } else {
    const roles =
      storageLocal().getItem<ConfigurationInfo>(userConfig)?.roles ?? [];
    const permissions =
      storageLocal().getItem<ConfigurationInfo>(userConfig)?.permissions ?? [];
    setUserConfig({
      roles,
      permissions
    });
  }
}

/** 删除`token`以及key值为`user-info`的localStorage信息 */
export function removeToken() {
  Cookies.remove(TokenKey);
  Cookies.remove(multipleTabsKey);
  storageLocal().removeItem(userKey);
  storageLocal().removeItem(userToken);
  storageLocal().removeItem(userConfig);
}

/** 格式化token（jwt格式） */
export const formatToken = (token: string): string => {
  return "Bearer " + token;
};

/** 是否有按钮级别的权限（根据登录接口返回的`permissions`字段进行判断）*/
export const hasPerms = (value: string | Array<string>): boolean => {
  if (!value) return false;
  const allPerms = "*:*:*";
  const { permissions } = useUserStoreHook();
  if (!permissions) return false;
  if (permissions.length === 1 && permissions[0] === allPerms) return true;
  const isAuths = isString(value)
    ? permissions.includes(value)
    : isIncludeAllChildren(value, permissions);
  return isAuths ? true : false;
};

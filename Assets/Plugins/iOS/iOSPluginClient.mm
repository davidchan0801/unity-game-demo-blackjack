//
//  iOSPluginClient.m
//  SlowmoCamController
//
//  Created by Forrest Chan on 26/9/2017.
//  Copyright © 2017 WowWee Group Limited. All rights reserved.
//

#include <sys/sysctl.h>
#import "iOSPluginClient.h"
#include <UnityFramework/UnityFramework.h>

@interface iOSPluginClient()

// tracker variable for when we add a contained view controller

@end

@implementation iOSPluginClient

static iOSPluginClient *_sharedPlugin = nil;

+ (iOSPluginClient *)sharedInstance {
    //    static dispatch_once_t oncePredicate;
    //    dispatch_once(&oncePredicate, ^
    //                  {
    //                      _sharedInstance = [[iOSPluginClient alloc] init];
    //                  });
    
    if(!_sharedPlugin) {
        _sharedPlugin = [iOSPluginClient new];
    }
    return _sharedPlugin;
}

- (id)init {
    self = [super init];
    
    //    [[NSNotificationCenter defaultCenter] addObserver:self selector:@selector(onMip2RobotFinderNotification:) name:Mip2RobotFinderNotificationID object:nil];
    
    return self;
}

- (void)showWebView:(NSString*)urlString {
    CGRect rect = [[UIScreen mainScreen] bounds];
    CGRect webViewRect = rect;
    webViewRect.size.height = rect.size.height + webViewRect.origin.y;
    webViewRect.origin.y = 0;
    view = [[WKWebView alloc] initWithFrame:webViewRect configuration:[[WKWebViewConfiguration alloc] init]];
    view.scrollView.contentInsetAdjustmentBehavior = UIScrollViewContentInsetAdjustmentNever;
    NSURL *url = [NSURL URLWithString:urlString];
    [view loadRequest:[NSURLRequest requestWithURL:url]];
    view.tag = 100000;
    [[self getTopViewController].view addSubview:view];
    
    UIButton *btn = [UIButton buttonWithType:UIButtonTypeClose];
    float scaleWidth = rect.size.width/1242.0f;
    float scaleHeight = rect.size.height/2668.0;
    float scale = 0;
    if (scaleWidth > scaleHeight)
        scale = scaleWidth;
    else
        scale = scaleHeight;
    btn.frame = CGRectMake(50.0f*scale, 50.0f*scale, 50.0f*scale, 50.0f*scale);
    
    [btn addTarget:self action:@selector(closeWebView) forControlEvents:UIControlEventTouchDown];
    [view addSubview:btn];
}

- (void)closeWebView {
    [view removeFromSuperview];
}

- (UIWindow*)getTopApplicationWindow {
    // grabs the top most window
    NSArray* windows = [[UIApplication sharedApplication] windows];
    return ([windows count] > 0) ? windows[0] : nil;
}

- (UIViewController*)getTopViewController {
    // get the top most window
    UIWindow *window = [self getTopApplicationWindow];
    
    // get the root view controller for the top most window
    UIViewController *vc = window.rootViewController;
    
    // check if this view controller has any presented view controllers, if so grab the top most one.
    while (vc.presentedViewController != nil) {
        // drill to topmost view controller
        vc = vc.presentedViewController;
    }
    
    return vc;
}
- (void)showNetworkLoading:(BOOL)shouldVisible {
    if (shouldVisible) {
        [self performSelectorOnMainThread:@selector(showNetworkLoading) withObject:nil waitUntilDone:YES];
    }
    else
        [self performSelectorOnMainThread:@selector(hideNetworkLoading) withObject:nil waitUntilDone:YES];
}

- (void)showNetworkLoading {
    if (loadingView == nil) {
        loadingView = [[UIView alloc] initWithFrame:[[UIScreen mainScreen] bounds]];
        [loadingView setBackgroundColor:[UIColor colorWithWhite:1.0f alpha:100.0f/255.0f]];
        indicatorView = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleLarge];
        indicatorView.center = CGPointMake([[UIScreen mainScreen] bounds].size.width/2, [[UIScreen mainScreen] bounds].size.height/2);
        [loadingView addSubview:indicatorView];
    }
    [indicatorView startAnimating];
    [[[[UnityFramework getInstance] appController] rootView] addSubview:loadingView];
    [[[[UnityFramework getInstance] appController] window] bringSubviewToFront:loadingView];
}

- (void)hideNetworkLoading {
    [indicatorView stopAnimating];
    [loadingView removeFromSuperview];
}

@end

/*******************************************************
 */
#pragma mark -    C String Helpers
/*
 ********************************************************/

// Converts C style string to NSString
NSString* CreateNSString (const char* string) {
    if (string) {
        return [NSString stringWithUTF8String: string];
    }
    else {
        return [NSString stringWithUTF8String: ""];
    }
}

NSArray* ExplodeNSStringFromCString (const char* string) {
    if (string) {
        return [[NSString stringWithUTF8String: string] componentsSeparatedByString:@","];
    }
    else {
        return [NSArray new];
    }
}

// Helper method to create C string copy
char* MakeStringCopy (const char* string) {
    if (string == NULL) {
        return NULL;
    }
    
    char* res = (char*)malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}



extern "C"
{
    // function definition, called from c# or javascript unity code
    void showWebview(char* urlString) {
        NSString *url = CreateNSString(urlString);
        [[iOSPluginClient sharedInstance] showWebView:url];
    }

    void showNetworkLoading(bool shouldVisible) {
        [[iOSPluginClient sharedInstance] showNetworkLoading:shouldVisible];
    }

}  // end of extern C block


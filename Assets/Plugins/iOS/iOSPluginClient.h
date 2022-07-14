//
//  iOSPluginClient.h
//  SlowmoCamController
//
//  Created by Forrest Chan on 26/9/2017.
//  Copyright © 2017 WowWee Group Limited. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <AVFoundation/AVFoundation.h>
#import <WebKit/WebKit.h>

@interface iOSPluginClient : NSObject {
    WKWebView *view;
    UIView *loadingView;
    UIActivityIndicatorView *indicatorView;
}

+ (iOSPluginClient *)sharedInstance;
- (void)showNetworkLoading:(BOOL)shouldVisible;
- (void)showWebView:(NSString*)urlString;

@end

